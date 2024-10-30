using Microsoft.EntityFrameworkCore;

using Quartz;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class SubscriptionPendingRenewalJob(SalesContext salesContext, IDeliveryDateCalculator deliveryDateCalculator, IBillingDateCalculator billingDateCalculator,
    TimeProvider timeProvider, ILogger<SubscriptionPendingRenewalJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Checking for pending renewals");

        int batchSize = 50;

        var subscriptions = await salesContext.Subscriptions
            .IgnoreQueryFilters()
            .Include(x => x.Plan)
            .Where(x => x.TypeId == 1 && x.StatusId == (int)SubscriptionStatusEnum.Active && x.RenewalOption == RenewalOption.Automatic && (x.RenewalStatus == RenewalStatus.None || x.RenewalStatus == RenewalStatus.RenewalPending))
            .Take(batchSize)
            .ToListAsync(context.CancellationToken);

        TimeSpan timeBeforeExpiration = TimeSpan.FromMinutes(5);

        foreach (var subscription in subscriptions)
        {
            var isEligibleForRenewal = subscription.SetRenewalPendingIfEligible(timeBeforeExpiration, billingDateCalculator, deliveryDateCalculator, timeProvider);

            if (isEligibleForRenewal)
            {
                logger.LogInformation("{SubscriptionId} is eligible for renewal", subscription.SubscriptionNo);
            }
            else
            {
                logger.LogInformation("{SubscriptionId} is NOT eligible for renewal: {Reason}", subscription.SubscriptionNo, subscription.GetIneligibilityReasonForRenewal(timeBeforeExpiration, timeProvider));
            }
        }

        await salesContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Finished checking for pending renewals");
    }
}