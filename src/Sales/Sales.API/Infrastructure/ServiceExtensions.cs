using Quartz;

using YourBrand.Domain.Infrastructure;
using YourBrand.Sales.Infrastructure.Jobs;
using YourBrand.Sales.Infrastructure.Services;

namespace YourBrand.Sales.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCoreServices();

        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddDomainInfrastructure(configuration);

        services.AddQuartz(configure =>
        {
            var jobKey = new JobKey(nameof(SubscriptionCancellationProcessor));

            int interval = 10;

            configure
                .AddJob<SubscriptionCancellationProcessor>(jobKey)
                .AddTrigger(trigger => trigger.ForJob(jobKey)
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInSeconds(interval)
                        .RepeatForever()));

            var jobKey2 = new JobKey(nameof(SubscriptionOrderGenerationProcess));

            configure
                .AddJob<SubscriptionOrderGenerationProcess>(jobKey2)
                .AddTrigger(trigger => trigger.ForJob(jobKey2)
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInSeconds(interval)
                        .RepeatForever()));

            var jobKey3 = new JobKey(nameof(SubscriptionPendingRenewalJob));

            configure
                .AddJob<SubscriptionPendingRenewalJob>(jobKey3)
                .AddTrigger(trigger => trigger.ForJob(jobKey3)
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInSeconds(15)
                        .RepeatForever()));

            var jobKey4 = new JobKey(nameof(SubscriptionAutoRenewalJob));

            configure
                .AddJob<SubscriptionAutoRenewalJob>(jobKey4)
                .AddTrigger(trigger => trigger.ForJob(jobKey4)
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInSeconds(20)
                        .RepeatForever()));

            var jobKey5 = new JobKey(nameof(SubscriptionActivationJob));

            configure
                .AddJob<SubscriptionActivationJob>(jobKey5)
                .AddTrigger(trigger => trigger.ForJob(jobKey5)
                    .WithSimpleSchedule(schedule => schedule
                        .WithIntervalInSeconds(10)
                        .RepeatForever()));
        });

        return services;
    }
}