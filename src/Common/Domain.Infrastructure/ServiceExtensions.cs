using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Quartz;

using YourBrand.Domain.Infrastructure.BackgroundJobs;
using YourBrand.Domain.Infrastructure.Idempotence;

namespace YourBrand.Domain.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddDomainInfrastructure<TDbContext>(this IServiceCollection services, IConfiguration configuration)
        where TDbContext : DbContext
    {
        services.AddScoped<IDomainDbContextAccessor>(sp => new DomainDbContextAccessor(sp.GetRequiredService<TDbContext>()));

        DecorateDomainEventHandlers(services);

        SetUpProcessOutboxMessagesJob<TDbContext>(services, configuration);

        services.AddDomainEventDispatcher();

        return services;
    }

    private static IServiceCollection AddDomainEventDispatcher(this IServiceCollection services)
    {
        services.TryAddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        return services;
    }

    private static void SetUpProcessOutboxMessagesJob<TDbContext>(IServiceCollection services, IConfiguration configuration)
        where TDbContext : DbContext
    {
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(typeof(ProcessOutboxMessagesJob<TDbContext>).Name);

            int interval = configuration.GetValue<int?>("ProcessOutboxMessagesJob:Interval") ?? 10;

            configure
                .AddJob<ProcessOutboxMessagesJob<TDbContext>>(jobKey)
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