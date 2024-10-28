using Microsoft.EntityFrameworkCore;

using Quartz;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Persistence;
using YourBrand.Sales.Persistence.Repositories.Mocks;

namespace YourBrand.Sales.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class SubscriptionOrderGenerationProcess(SalesContext salesContext, IDeliveryDateCalculator deliveryDateCalculator, 
    TimeProvider timeProvider, ILogger<SubscriptionOrderGenerationProcess> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        int batchSize = 50;

        var subscriptions = await salesContext.Subscriptions
            .IgnoreQueryFilters()
            .Include(x => x.Plan)
            .Where(x => x.TypeId == 1 && x.StatusId == (int)SubscriptionStatusEnum.Active)
            .Take(batchSize)
            .ToListAsync(context.CancellationToken);

        foreach(var subscription in subscriptions) 
        {
            var order = await salesContext.Orders
                .IncludeAll()
                .FirstOrDefaultAsync(x => x.Id == subscription.OrderId, context.CancellationToken);

            if(order is not null) 
            {
                DateTimeOffset now = timeProvider.GetUtcNow();

                var nextDeliveryDate = deliveryDateCalculator.CalculateNextDeliveryDate(subscription, now);

                logger.LogInformation("Next delivery date for {SubscriptionId} is {Date}", subscription.SubscriptionNo, nextDeliveryDate);
            }
        }
    }
}
