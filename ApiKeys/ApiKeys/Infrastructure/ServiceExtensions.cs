
using YourBrand.ApiKeys.Infrastructure.Persistence.Interceptors;

using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Infrastructure.Persistence;
using YourBrand.ApiKeys.Infrastructure.Services;
using Quartz;
using YourBrand.ApiKeys.Infrastructure.BackgroundJobs;
using MediatR;
using YourBrand.ApiKeys.Infrastructure.Idempotence;
using Scrutor;

namespace YourBrand.ApiKeys.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddQuartz(configure =>
            {
                var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

                configure
                    .AddJob<ProcessOutboxMessagesJob>(jobKey)
                    .AddTrigger(trigger => trigger.ForJob(jobKey)
                        .WithSimpleSchedule(schedule => schedule
                            .WithIntervalInSeconds(10)
                            .RepeatForever()));

                configure.UseMicrosoftDependencyInjectionJobFactory();
            });

            services.AddQuartzHostedService();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServer<ApiKeysContext>(
            configuration.GetConnectionString("mssql", "ApiKeys") ?? configuration.GetConnectionString("DefaultConnection"),
            options => options.EnableRetryOnFailure());

        services.AddScoped<IApiKeysContext>(sp => sp.GetRequiredService<ApiKeysContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        try 
        {
            services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
        }
        catch(DecorationException exc) when (exc.Message.Contains("Could not find any registered services for type"))
        {
            Console.WriteLine(exc);
        }

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}