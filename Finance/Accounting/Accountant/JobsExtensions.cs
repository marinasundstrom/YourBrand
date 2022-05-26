using System;

using Accountant.Services;

using Hangfire;

namespace Accountant;

public static class JobsExtensions
{
    public static IServiceProvider InitializeJobs(this IServiceProvider services)
    {
        var backgroundJobs = services.GetRequiredService<IBackgroundJobClient>();
        backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

        var recurringJobs = services.GetRequiredService<IRecurringJobManager>();
        recurringJobs.AddOrUpdate<RefundService>("refund", (refundService) => refundService.CheckForRefund(), Cron.Minutely());
        recurringJobs.AddOrUpdate<IReminderService>("remind", (reminderService) => reminderService.IssueReminders(), Cron.MinuteInterval(10));

        return services;
    }
}