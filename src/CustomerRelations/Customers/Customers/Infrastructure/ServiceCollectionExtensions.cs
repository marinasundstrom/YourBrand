using MediatR;

using Quartz;

using Scrutor;

using YourBrand.Customers.Application.Common.Interfaces;
using YourBrand.Customers.Infrastructure.BackgroundJobs;
using YourBrand.Customers.Infrastructure.Idempotence;
using YourBrand.Customers.Infrastructure.Persistence;
using YourBrand.Customers.Infrastructure.Services;

namespace YourBrand.Customers.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

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
}