using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;
using Scrutor;

using YourBrand.Ticketing.Infrastructure.BackgroundJobs;
using YourBrand.Ticketing.Infrastructure.Idempotence;
using YourBrand.Ticketing.Infrastructure.Persistence;
using YourBrand.Ticketing.Infrastructure.Services;

namespace YourBrand.Ticketing.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddScoped<IEmailService, EmailService>();

        RemoveFaultyDomainEventHandlerRegistrations(services);

        try
        {
            services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
        }
        catch (DecorationException exc) when (exc.Message.Contains("Could not find any registered services for type"))
        {
            Console.WriteLine(exc);
        }
        
        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

            configure
                .AddJob<ProcessOutboxMessagesJob>(jobKey)
                .AddTrigger(trigger => trigger.ForJob(jobKey)
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInSeconds(10)
                        .RepeatForever()));
        });

        services.AddQuartzHostedService();

        return services;
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