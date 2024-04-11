using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;

using YourBrand.Domain.Infrastructure.BackgroundJobs;
using YourBrand.Domain.Infrastructure.Idempotence;

namespace YourBrand.Domain.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddDomainInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        DecorateDomainEventHandlers(services);

        SetUpProcessOutboxMessagesJob(services, configuration);

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }

    private static void SetUpProcessOutboxMessagesJob(IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

            int interval = configuration.GetValue<int?>("ProcessOutboxMessagesJob:Interval") ?? 10;

            configure
                .AddJob<ProcessOutboxMessagesJob>(jobKey)
                .AddTrigger(trigger => trigger.ForJob(jobKey)
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInSeconds(interval)
                        .RepeatForever()));
        });

        services.AddQuartzHostedService();
    }

    private static void DecorateDomainEventHandlers(IServiceCollection services)
    {
        //RemoveFaultyDomainEventHandlerRegistrations(services);

        try
        {
            services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
        }
        catch { }
    }

    private static void RemoveFaultyDomainEventHandlerRegistrations(IServiceCollection services)
    {
        // This removes registrations between INotificationHandler<T> to IdempotentDomainEventHandler<T>. This was not a problem before MediatR 12.0.
        // An alternative would be to put IdempotentDomainEventHandler in a library separate from Application logic, so that MediatR doesn't register that implementation as a real handler.

        foreach (var reg in services.Where(reg => reg.ServiceType.Name.Contains("INotificationHandler")).ToList())
        {
            var notificationHandlerType = reg.ServiceType!;
            var notificationHandlerImplType = reg.ImplementationType!;

            var requestType = notificationHandlerType.GetGenericArguments().FirstOrDefault();

            if (!notificationHandlerImplType.Name.Contains("IdempotentDomainEventHandler")) continue;

            services.Remove(reg);
        }
    }
}