using Microsoft.EntityFrameworkCore;

using Quartz;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class SubscriptionAutoRenewalJob(SalesContext salesContext, IDeliveryDateCalculator deliveryDateCalculator, IBillingDateCalculator billingDateCalculator,
    TimeProvider timeProvider, ILogger<SubscriptionAutoRenewalJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Processing auto-renewals");

        int batchSize = 50;

        var subscriptions = await salesContext.Subscriptions
            .IgnoreQueryFilters()
            .Include(x => x.Plan)
            .Where(x => x.TypeId == 1 && x.StatusId == (int)SubscriptionStatusEnum.Active && x.RenewalStatus == RenewalStatus.RenewalPending)
            .Take(batchSize)
            .ToListAsync(context.CancellationToken);

        foreach (var subscription in subscriptions)
        {
            var wasRenewed = subscription.Renew(billingDateCalculator, deliveryDateCalculator, timeProvider);

            if (wasRenewed)
            {
                logger.LogInformation("{SubscriptionId} was renewed", subscription.SubscriptionNo);
            }
        }

        await salesContext.SaveChangesAsync(context.CancellationToken);

        logger.LogInformation("Finished auto-renewals");
    }
}