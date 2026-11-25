using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Quartz;

using YourBrand.TimeReport.Infrastructure.BackgroundJobs;
using YourBrand.TimeReport.Infrastructure.Persistence;

namespace YourBrand.TimeReport.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey(nameof(ProcessOutboxMessagesJob));

            q.AddJob<ProcessOutboxMessagesJob>(job =>
                job.WithIdentity(jobKey));

            q.AddTrigger<ProcessOutboxMessagesJob>(trigger => trigger
                .WithIdentity($"{jobKey.Name}-trigger")
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(10))
                    .RepeatForever()));
        });

        services.AddQuartzHostedService();

        return services;
    }
}