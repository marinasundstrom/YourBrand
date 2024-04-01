using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.Subscriptions;

public record GenerateSubscriptionOrders(string OrderId) : IRequest
{
    public class Handler(SalesContext salesContext, SubscriptionOrderGenerator subscriptionOrderGenerator) : IRequestHandler<GenerateSubscriptionOrders>
    {
        public async Task Handle(GenerateSubscriptionOrders request, CancellationToken cancellationToken)
        {
            var order = await salesContext.Orders
                .Include(x => x.Subscription)
                .ThenInclude(x => x.SubscriptionPlan)
                .Include(x => x.Items)
                .ThenInclude(x => x.Subscription)
                .ThenInclude(x => x.SubscriptionPlan)
                .FirstOrDefaultAsync(c => c.Id == request.OrderId);

            if (order is null)
            {
                throw new System.Exception();
            }

            var orders = subscriptionOrderGenerator.GetOrders(order, DateTime.Now, DateTime.Now.AddDays(25));

            var orderNo = (await salesContext.Orders.MaxAsync(x => x.OrderNo)) + 1;

            foreach (var order2 in orders)
            {
                try
                {
                    order2.OrderNo = orderNo++;
                }
                catch (InvalidOperationException e)
                {
                    order2.OrderNo = 1; // Order start number
                }

                salesContext.Orders.Add(order2);
            }

            await salesContext.SaveChangesAsync();
        }
    }
}