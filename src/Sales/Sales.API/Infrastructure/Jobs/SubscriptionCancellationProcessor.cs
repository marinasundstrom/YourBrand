using Microsoft.EntityFrameworkCore;

using Quartz;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class SubscriptionCancellationProcessor(SalesContext salesContext, TimeProvider timeProvider) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await ProcessPendingCancellations(context.CancellationToken);
    }

    public async Task ProcessPendingCancellations(CancellationToken cancellationToken)
    {
        var currentTime = timeProvider.GetUtcNow();

        // Fetch subscriptions that have a cancellation request pending and are eligible for finalization
        var subscriptionsToCancel = await GetPendingCancellationsAsync(currentTime, cancellationToken);

        foreach (var subscription in subscriptionsToCancel)
        {
            try
            {
                subscription.FinalizeCancellation(timeProvider);
                await salesContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error finalizing cancellation for subscription {subscription.Id}: {ex.Message}");
            }
        }
    }

    public async Task<List<Subscription>> GetPendingCancellationsAsync(DateTimeOffset currentTime, CancellationToken cancellationToken)
    {
        int batchSize = 50;

        // Step 1: Retrieve all subscriptions with CancellationRequested and necessary dates set
        var subscriptions = await salesContext.Subscriptions
            .IgnoreQueryFilters()
            .Where(subscription =>
                subscription.CancellationStatus == CancellationStatus.CancellationRequested &&
                subscription.CancellationRequestedDate.HasValue &&
                subscription.CancellationFinalizationPeriod.HasValue)
            .OrderByDescending(x => x.CancellationRequestedDate)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        // Step 2: Filter in memory to check if the finalization period has passed
        var subscriptionsToCancel = subscriptions
            .Where(subscription =>
                subscription.CancellationRequestedDate.Value.Add(subscription.CancellationFinalizationPeriod.Value) <= currentTime)
            .ToList();

        return subscriptionsToCancel;
    }
}