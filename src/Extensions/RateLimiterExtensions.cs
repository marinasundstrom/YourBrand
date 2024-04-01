using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace YourBrand.Extensions;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddRateLimiterForIPAddress(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MyRateLimitOptions>(
        configuration.GetSection(MyRateLimitOptions.MyRateLimit));

        var myOptions = new MyRateLimitOptions();
        configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);

        services.AddRateLimiter(options =>
        {
            options.OnRejected = (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter =
                        ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                }

                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.RequestServices.GetService<ILoggerFactory>()?
                    .CreateLogger("Microsoft.AspNetCore.RateLimitingMiddleware")
                    .LogWarning("OnRejected: {GetUserEndPoint}", GetUserEndPoint(context.HttpContext));

                return new ValueTask();
            };

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, IPAddress>(context =>
            {
                IPAddress? remoteIpAddress = context.Connection.RemoteIpAddress;

                if (!IPAddress.IsLoopback(remoteIpAddress!))
                {
                    return RateLimitPartition.GetTokenBucketLimiter
                    (remoteIpAddress!, _ =>
                        new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = myOptions.TokenLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = myOptions.QueueLimit,
                            ReplenishmentPeriod = TimeSpan.FromSeconds(myOptions.ReplenishmentPeriod),
                            TokensPerPeriod = myOptions.TokensPerPeriod,
                            AutoReplenishment = myOptions.AutoReplenishment
                        });
                }

                return RateLimitPartition.GetNoLimiter(IPAddress.Loopback);
            });
        });

        static string GetUserEndPoint(HttpContext context) =>
            $"User {context.User.Identity?.Name ?? "Anonymous"} endpoint:{context.Request.Path}"
            + $" {context.Connection.RemoteIpAddress}";

        return services;
    }
}

public class MyRateLimitOptions
{
    public static readonly string MyRateLimit = "MyRateLimit";

    public int TokenLimit { get; set; } = 5;
    public int QueueLimit { get; set; } = 0;
    public double ReplenishmentPeriod { get; set; } = 10;
    public int TokensPerPeriod { get; set; } = 5;
    public bool AutoReplenishment { get; set; } = true;
}