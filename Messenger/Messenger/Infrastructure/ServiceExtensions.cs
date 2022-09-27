
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Infrastructure.Persistence;
using YourBrand.Messenger.Infrastructure.Persistence.Interceptors;
using YourBrand.Messenger.Infrastructure.Services;
using Quartz;
using YourBrand.Messenger.Infrastructure.BackgroundJobs;

namespace YourBrand.Messenger.Infrastructure;

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
}