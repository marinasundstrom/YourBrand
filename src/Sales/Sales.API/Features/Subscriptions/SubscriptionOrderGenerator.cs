using System;
using System.Collections.Generic;
using System.Linq;

using YourBrand.Sales.Features.Orders;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Features.OrderManagement.Domain.Entities;

namespace YourBrand.Sales.Features.Subscriptions;

public class SubscriptionOrderGenerator(
    OrderFactory orderFactory,
    SubscriptionOrderDateGenerator orderDateGenerator)
{
    public IEnumerable<Order> GetOrders(Order order, DateTime? startDate = null, DateTime? endDate = null)
    {
        if (order.Subscription is null)
        {
            // Order (Non-Subscription Order)

            var subOrder = orderFactory.CreateOrder(order);
            //.SetDeliveryDate(date.Start, date.End);

            foreach (var orderItem in order.Items)
            {
                /* if (orderItem.Subscription is null && orderItem.HasDeliveryDetails)
                {
                    // Deliver to separate address. Its own Delivery.
                    var subOrder2 = orderFactory.CreateOrder(orderItem);
                    subOrder2.Items.Add(
                        orderFactory.CreateOrderItem(orderItem));
                    yield return subOrder2;
                }
                else */

                if (orderItem.Subscription is not null)
                {
                    // Subscription Item with multiple Deliveries

                    var dates = orderDateGenerator.GetOrderDatesFromSubscription(orderItem.Subscription, startDate, endDate);

                    foreach (var date in dates)
                    {
                        var subOrder2 = orderFactory
                            .CreateOrderFromOrderItem(orderItem)
                            .SetPlannedDates(date.Start, date.End);

                        subOrder2.AddItem(
                            orderFactory.CreateOrderItem(orderItem));

                        yield return subOrder2;
                    }
                }

                /*
                else
                {
                    // Item part of Main Delivery
                    var targetOrderItem = orderFactory.CreateOrderItem(orderItem);
                    subOrder.Items.Add(targetOrderItem);
                }
                */
            }

            // Return Main Delivery if any items
            if (subOrder.Items.Any())
            {
                yield return subOrder;
            }
        }
        else
        {
            // Subscription Order

            var dates = orderDateGenerator.GetOrderDatesFromSubscription(order.Subscription, startDate, endDate);

            foreach (var date in dates)
            {
                var subOrder = orderFactory.CreateOrder(order)
                    .SetPlannedDates(date.Start, date.End);

                foreach (var orderItem in order.Items)
                {
                    /*
                    if (orderItem.SubscriptionPlan is not null)
                    {
                        throw new Exception($"Nested subscriptions not allowed (OrderItem.Id: {orderItem.Id})");
                    }
                    */

                    /*
                    if (orderItem.HasDeliveryDetails)
                    {
                        // Deliver to separate address. Its own Delivery.

                        var subOrder2 = orderFactory
                          .CreateOrderFromOrderItem(orderItem)
                          .SetPlannedDates(date.Start, date.End);

                        subOrder2.AddItem(
                            orderFactory.CreateOrderItem(orderItem));

                        yield return subOrder2;
                    }
                    else
                    { */
                        // Item part of Main Delivery

                        var targetOrderItem = orderFactory.CreateOrderItem(orderItem);

                        subOrder.AddItem(targetOrderItem);
                    //}
                }

                // Return Main Delivery if any items
                if (subOrder.Items.Any())
                {
                    yield return subOrder;
                }
            }
        }
    }
}