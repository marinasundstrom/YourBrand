using Quartz;

using YourBrand.Domain.Infrastructure;
using YourBrand.Sales.Infrastructure.Jobs;
using YourBrand.Sales.Infrastructure.Services;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCoreServices();

        services.AddScoped<IEmailService, EmailService>();

        services.AddDomainInfrastructure<SalesContext>(configuration);

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey(nameof(SubscriptionCancellationProcessor));

            q.AddJob<SubscriptionCancellationProcessor>(job =>
                job.WithIdentity(jobKey));

            q.AddTrigger<SubscriptionCancellationProcessor>(trigger => trigger
                .WithIdentity($"{jobKey.Name}-trigger")
                .ForJob(jobKey)
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(10))
                    .RepeatForever()));

            var jobKey2 = new JobKey(nameof(SubscriptionOrderGenerationProcess));

            q.AddJob<SubscriptionOrderGenerationProcess>(job =>
                job.WithIdentity(jobKey2));

            q.AddTrigger<SubscriptionOrderGenerationProcess>(trigger => trigger
                .WithIdentity($"{jobKey2.Name}-trigger")
                .ForJob(jobKey2)
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(10))
                    .RepeatForever()));

            var jobKey3 = new JobKey(nameof(SubscriptionPendingRenewalJob));

            q.AddJob<SubscriptionPendingRenewalJob>(job =>
                job.WithIdentity(jobKey3));

            q.AddTrigger<SubscriptionPendingRenewalJob>(trigger => trigger
                .WithIdentity($"{jobKey3.Name}-trigger")
                .ForJob(jobKey3)
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(15))
                    .RepeatForever()));

            var jobKey4 = new JobKey(nameof(SubscriptionAutoRenewalJob));

            q.AddJob<SubscriptionAutoRenewalJob>(job =>
                job.WithIdentity(jobKey4));

            q.AddTrigger<SubscriptionAutoRenewalJob>(trigger => trigger
                .WithIdentity($"{jobKey4.Name}-trigger")
                .ForJob(jobKey4)
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(20))
                    .RepeatForever()));

            var jobKey5 = new JobKey(nameof(SubscriptionActivationJob));

            q.AddJob<SubscriptionActivationJob>(job =>
                job.WithIdentity(jobKey5));

            q.AddTrigger<SubscriptionActivationJob>(trigger => trigger
                .WithIdentity($"{jobKey5.Name}-trigger")
                .ForJob(jobKey5)
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(10))
                    .RepeatForever()));
        });

        return services;
    }
}