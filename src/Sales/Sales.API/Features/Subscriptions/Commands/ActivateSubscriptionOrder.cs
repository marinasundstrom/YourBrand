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

            if (order.OrderNo is null)
            {
                await order.AssignOrderNo(orderNumberFetcher, cancellationToken);
            }

            var orderNo = order.OrderNo.GetValueOrDefault();

            foreach (var order2 in orders)
            {
                order2.AssignOrderNo(++orderNo);

                order2.OrganizationId = request.OrganizationId;
                order2.TypeId = 3;

                order2.UpdateStatus((int)OrderStatusEnum.Planned, timeProvider);

                salesContext.Orders.Add(order2);
            }

            //order.Subscription.Order = order;

            var plan = subscription.Plan;

            if (plan is null && plan.Trial.HasTrial)
            {
                // Activate trial
                subscription.StartTrial(plan.Trial.Length, timeProvider);
            }
            else
            {
                // Activate subscription
                subscription.Activate(timeProvider);
            }

            // Complete subscription order
            order.Complete(timeProvider);

            await salesContext.SaveChangesAsync();
        }
    }
}