using MediatR;

using Quartz;

using Scrutor;

using YourBrand.Inventory.Application.Common.Interfaces;
using YourBrand.Inventory.Infrastructure.BackgroundJobs;
using YourBrand.Inventory.Infrastructure.Idempotence;
using YourBrand.Inventory.Infrastructure.Persistence;
using YourBrand.Inventory.Infrastructure.Services;

namespace YourBrand.Inventory.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        RemoveFaultyDomainEventHandlerRegistrations(services);

        try
        {
            services.Decorate(typeof(INotificationHandler<>), typeof(IdempotentDomainEventHandler<>));
        }
        catch (DecorationException exc) when (exc.Message.Contains("Could not find any registered services for type"))
        {
            Console.WriteLine(exc);
        }

        services.AddTransient<IDateTime, DateTimeService>();

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