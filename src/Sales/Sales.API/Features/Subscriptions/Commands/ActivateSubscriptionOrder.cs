using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Orders.Commands;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public record ActivateSubscriptionOrder(string OrganizationId, string OrderId) : IRequest
{
    public class Handler(SalesContext salesContext, OrderNumberFetcher orderNumberFetcher, SubscriptionOrderGenerator subscriptionOrderGenerator) : IRequestHandler<ActivateSubscriptionOrder>
    {
        public async Task Handle(ActivateSubscriptionOrder request, CancellationToken cancellationToken)
        {
            var subscription = await salesContext.Subscriptions
                .FirstOrDefaultAsync(c => c.OrderId == request.OrderId);

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

            if(order.StatusId != 4) 
            {
                throw new System.Exception("Order not confirmed");
            }

            var orders = subscriptionOrderGenerator.GenerateOrders(order, subscription.StartDate, subscription.EndDate);

            var orderNo = await orderNumberFetcher.GetNextNumberAsync(request.OrganizationId, cancellationToken);

            foreach (var order2 in orders)
            {
                try
                {
                    order2.OrderNo = orderNo++;
                    order2.OrganizationId = request.OrganizationId;
                    order2.TypeId = 3;

                    order2.UpdateStatus(2);
                }
                catch (InvalidOperationException e)
                {
                    order2.OrderNo = 1; // Order start number
                }

                salesContext.Orders.Add(order2);
            }

            order.UpdateStatus(12);

            //order.Subscription.Order = order;
            subscription.UpdateStatus(2);

            await salesContext.SaveChangesAsync();
        }
    }
}