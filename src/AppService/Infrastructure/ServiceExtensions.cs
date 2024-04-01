using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;

using Scrutor;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Infrastructure.BackgroundJobs;
using YourBrand.Infrastructure.Idempotence;
using YourBrand.Infrastructure.Persistence;
using YourBrand.Infrastructure.Persistence.Interceptors;
using YourBrand.Infrastructure.Services;

namespace YourBrand.Infrastructure;

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
        services.AddSqlServer<AppServiceContext>(
            configuration.GetConnectionString("mssql", "AppService") ?? configuration.GetConnectionString("DefaultConnection"),
            options => options.EnableRetryOnFailure());

        services.AddScoped<IAppServiceContext>(sp => sp.GetRequiredService<AppServiceContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        try
        {
            services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
        }
        catch (DecorationException exc) when (exc.Message.Contains("Could not find any registered services for type"))
        {
            Console.WriteLine(exc);
        }

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}