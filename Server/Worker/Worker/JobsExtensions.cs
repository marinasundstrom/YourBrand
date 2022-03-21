
using Hangfire;

using Worker.Services;

public static class JobsExtensions
{
    public static IServiceProvider InitializeJobs(this IServiceProvider services)
    {
        var backgroundJobs = services.GetRequiredService<IBackgroundJobClient>();
        backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

        var recurringJobs = services.GetRequiredService<IRecurringJobManager>();
        recurringJobs.AddOrUpdate<INotifier>("test", (notifier) => notifier.Notify(), Cron.Minutely());

        return services;
    }
}