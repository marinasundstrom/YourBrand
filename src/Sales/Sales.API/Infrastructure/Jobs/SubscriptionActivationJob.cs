using Microsoft.EntityFrameworkCore;

using Quartz;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class SubscriptionActivationJob(SalesContext salesContext,
    TimeProvider timeProvider, ILogger<SubscriptionActivationJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Checking for pending activations");

        int batchSize = 50;

        var currentDate = timeProvider.GetUtcNow();

        var candidateSubscriptions = await salesContext.Subscriptions
            .IgnoreQueryFilters()
            .Include(x => x.Plan)
            .Where(subscription =>
                subscription.StatusId == (int)SubscriptionStatusEnum.Trial && // Subscription is in trial state
                subscription.TrialEndDate.HasValue && // Ensure there is a trial end date
                subscription.TrialEndDate.Value <= currentDate && // Trial has ended
                (subscription.PaymentStatus == PaymentStatus.PaymentSucceeded || // Payment succeeded
                subscription.PaymentStatus == PaymentStatus.PaymentFailed) // or Payment failed
            )
            .Take(batchSize)
            .ToListAsync(context.CancellationToken);

        foreach (var subscription in candidateSubscriptions)
        {
            // Handle activation based on payment status
            if (subscription.HasPaymentSucceeded)
            {
                subscription.Activate(timeProvider);
            }
            else if (subscription.HasPaymentFailed)
            {
                // Decide to cancel or suspend based on business rules
                if (subscription.ShouldCancelOnTrialEnd())
                {
                    subscription.Cancel("Payment failed at trial end", "System", timeProvider);
                }
                else
                {
                    subscription.Suspend(timeProvider);
                }
            }
        }

        await salesContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Finished checking for subscriptions to activate");
    }
}