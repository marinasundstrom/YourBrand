using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using Aspire.Hosting.Lifecycle;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

// An annotation that associates a health check factory with a resource
public class HealthCheckAnnotation(Func<string, IHealthCheck> healthCheckFactory) : IResourceAnnotation
{
    public Func<string, IHealthCheck> HealthCheckFactory { get; } = healthCheckFactory;
}

public static class WaitForDependenciesExtensions
{
    /// <summary>
    /// Wait for a resource to be running before starting another resource.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static IResourceBuilder<T> WaitOn<T>(this IResourceBuilder<T> builder, IResourceBuilder<IResource> other)
        where T : IResource
    {
        builder.ApplicationBuilder.AddWaitForDependencies();
        return builder.WithAnnotation(new WaitOnAnnotation(other.Resource));
    }

    /// <summary>
    /// Adds a lifecycle hook that waits for all dependencies to be "running" before starting resources. If that resource
    /// has a health check, it will be executed before the resource is considered "running".
    /// </summary>
    /// <param name="builder">The <see cref="IDistributedApplicationBuilder"/>.</param>
    private static IDistributedApplicationBuilder AddWaitForDependencies(this IDistributedApplicationBuilder builder)
    {
        builder.Services.TryAddLifecycleHook<WaitForDependenciesRunningHook>();
        return builder;
    }

    private class WaitOnAnnotation(IResource resource) : IResourceAnnotation
    {
        public IResource Resource { get; } = resource;
    }

    private class WaitForDependenciesRunningHook(DistributedApplicationExecutionContext executionContext,
        ResourceNotificationService resourceNotificationService) :
        IDistributedApplicationLifecycleHook,
        IAsyncDisposable
    {
        private readonly CancellationTokenSource _cts = new();

        public Task BeforeStartAsync(DistributedApplicationModel appModel, CancellationToken cancellationToken = default)
        {
            // We don't need to execute any of this logic in publish mode
            if (executionContext.IsPublishMode)
            {
                return Task.CompletedTask;
            }

            // The global list of resources being waited on
            var waitingResources = new ConcurrentDictionary<IResource, TaskCompletionSource>();

            // For each resource, add an environment callback that waits for dependencies to be running
            foreach (var r in appModel.Resources)
            {
                var resourcesToWaitOn = r.Annotations.OfType<WaitOnAnnotation>().Select(a => a.Resource).Distinct().ToArray();

                if (resourcesToWaitOn.Length == 0)
                {
                    continue;
                }

                // Abuse the environment callback to wait for dependencies to be running

                r.Annotations.Add(new EnvironmentCallbackAnnotation(async context =>
                {
                    var dependencies = new List<Task>();

                    // Find connection strings and endpoint references and get the resource they point to
                    foreach (var resource in resourcesToWaitOn)
                    {
                        // REVIEW: This logic does not handle cycles in the dependency graph (that would result in a deadlock)

                        // Don't wait for yourself
                        if (resource != r && resource is not null)
                        {
                            context.Logger?.LogInformation("Waiting for {Resource} to be running", resource.Name);

                            dependencies.Add(waitingResources.GetOrAdd(resource, _ => new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)).Task);
                        }
                    }

                    await Task.WhenAll(dependencies).WaitAsync(context.CancellationToken);
                }));
            }

            _ = Task.Run(async () =>
           {
               var stoppingToken = _cts.Token;

               // Watch for global resource state changes
               await foreach (var resourceEvent in resourceNotificationService.WatchAsync(stoppingToken))
               {
                   // These states are terminal but we need a better way to detect that
                   if (resourceEvent.Snapshot.State == "FailedToStart" ||
                       resourceEvent.Snapshot.State == "Exited" ||
                       resourceEvent.Snapshot.ExitCode is not null)
                   {
                       if (waitingResources.TryRemove(resourceEvent.Resource, out var tcs))
                       {
                           tcs.TrySetCanceled();
                       }
                   }
                   else if (resourceEvent.Snapshot.State == "Running" && waitingResources.TryRemove(resourceEvent.Resource, out var tcs))
                   {
                       _ = DoTheHealthCheck(resourceEvent, tcs);
                   }
               }
           },
           cancellationToken);

            return Task.CompletedTask;
        }

        private async Task DoTheHealthCheck(ResourceEvent resourceEvent, TaskCompletionSource tcs)
        {
            var url = resourceEvent.Snapshot.Urls.Select(u => new Uri(u.Url)).FirstOrDefault();

            resourceEvent.Resource.TryGetLastAnnotation<HealthCheckAnnotation>(out var healthCheckAnnotation);

            Func<CancellationToken, ValueTask>? operation = (url?.Scheme, healthCheckAnnotation?.HealthCheckFactory) switch
            {
                ("http" or "https", null) => async (ct) =>
                {
                    // For an HTTP resource, see if we can make a request to the endpoint
                    using var client = new HttpClient();
                    var response = await client.GetAsync(url, ct);
                }
                ,
                (_, Func<string, IHealthCheck> factory) => async (ct) =>
                {
                    if (resourceEvent.Resource is not IResourceWithConnectionString c)
                    {
                        return;
                    }

                    // Get the connection string from the resource so we can create the health check
                    // with the correct connection information

                    // TODO: We could cache this lookup
                    var cs = await c.GetConnectionStringAsync(ct);

                    if (cs is null)
                    {
                        return;
                    }

                    var check = factory(cs);

                    var context = new HealthCheckContext()
                    {
                        Registration = new HealthCheckRegistration("", check, HealthStatus.Unhealthy, [])
                    };

                    var result = await check.CheckHealthAsync(context, ct);

                    if (result.Exception is not null)
                    {
                        ExceptionDispatchInfo.Throw(result.Exception);
                    }

                    if (result.Status != HealthStatus.Healthy)
                    {
                        throw new Exception("Health check failed");
                    }
                }
                ,
                _ => null,
            };

            try
            {
                if (operation is not null)
                {
                    await ResiliencePipeline.ExecuteAsync(operation);
                }

                tcs.TrySetResult();
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        }

        private ResiliencePipeline? _resiliencePipeline;
        private ResiliencePipeline ResiliencePipeline => _resiliencePipeline ??= CreateResiliencyPipeline();

        private static ResiliencePipeline CreateResiliencyPipeline()
        {
            var retryUntilCancelled = new RetryStrategyOptions()
            {
                ShouldHandle = new PredicateBuilder().HandleInner<Exception>(),
                BackoffType = DelayBackoffType.Exponential,
                MaxRetryAttempts = 5,
                UseJitter = true,
                MaxDelay = TimeSpan.FromSeconds(30)
            };

            return new ResiliencePipelineBuilder().AddRetry(retryUntilCancelled).Build();
        }

        public ValueTask DisposeAsync()
        {
            _cts.Cancel();
            return default;
        }
    }
}