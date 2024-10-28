using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Orders.Commands;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public record ActivateSubscriptionOrder(string OrganizationId, string OrderId) : IRequest
{
    public class Handler(SalesContext salesContext, OrderNumberFetcher orderNumberFetcher,
        TimeProvider timeProvider,
        SubscriptionOrderGenerator subscriptionOrderGenerator) : IRequestHandler<ActivateSubscriptionOrder>
    {
        public async Task Handle(ActivateSubscriptionOrder request, CancellationToken cancellationToken)
        {
            var subscription = await salesContext.Subscriptions
                .FirstOrDefaultAsync(c => c.OrderId == request.OrderId);

            var order = await salesContext.Orders
                .Include(x => x.Subscription)
                .ThenInclude(x => x.Plan)
                .Include(x => x.Items)
                .ThenInclude(x => x.Subscription)
                .ThenInclude(x => x.Plan)
                .FirstOrDefaultAsync(c => c.Id == request.OrderId);

            if (order is null)
            {
                throw new System.Exception();
            }

            if (order.StatusId != (int)OrderStatusEnum.Confirmed)
            {
                throw new System.InvalidOperationException("Order not confirmed");
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

                    order2.UpdateStatus((int)OrderStatusEnum.Planned);
                }
                catch (InvalidOperationException e)
                {
                    order2.OrderNo = 1; // Order start number
                }

                salesContext.Orders.Add(order2);
            }

            //order.Subscription.Order = order;

            var plan = subscription.Plan;

            if (plan is null && plan.HasTrial)
            {
                // Activate trial
                subscription.StartTrial(plan.TrialLength, timeProvider);
            }
            else
            {
                // Activate subscription
                subscription.Activate(timeProvider);
            }

            // Complete subscription order
            order.Complete();

            await salesContext.SaveChangesAsync();
        }
    }
}