using System;

using YourBrand.Orders.Domain.Entities;

namespace YourBrand.Orders.Application.Orders;

public class OrderFactory
{
    public Order CreateOrder(Order order)
    {
        var targetOrder = new Order();
        UpdateOrder(targetOrder, order);
        return targetOrder;
    }

    public Order CreateOrderFromOrderItem(OrderItem orderItem)
    {
        var targerOrder = new Order();
        UpdateOrder(targerOrder, orderItem);
        return targerOrder;
    }

    public OrderItem CreateOrderItem(OrderItem orderItem)
    {
        var targetOrderItem = new OrderItem();
        UpdateOrderItem(targetOrderItem, orderItem);
        return targetOrderItem;
    }

    public void UpdateOrder(Order targetOrder, Order order)
    {
        targetOrder.StatusId = "approved";
        targetOrder.StatusDate = DateTime.Now;

        //targetOrder.Order = order;
        targetOrder.Subscription = order.Subscription;
        targetOrder.Note = order?.Note;
        targetOrder.DeliveryDetails = order?.DeliveryDetails?.Clone();
        //delivery.Assignee = order?.Assignee;
        targetOrder.Created = DateTime.Now;
    }

    public void UpdateOrder(Order targetOrder, OrderItem orderItem)
    {
        targetOrder.StatusId = "approved";
        targetOrder.StatusDate = DateTime.Now;

        //targetOrder.Order = orderItem.Order;
        targetOrder.Subscription = orderItem.Subscription ?? orderItem.Order.Subscription;
        targetOrder.DeliveryDetails = orderItem.HasDeliveryDetails ? orderItem?.DeliveryDetails?.Clone() : orderItem?.Order?.DeliveryDetails?.Clone();
        //delivery.Assignee = orderItem?.Assignee ?? orderItem?.Order?.Assignee;
        targetOrder.Note = orderItem!.Note;

        targetOrder.Created = DateTime.Now;
    }

    public void UpdateOrderItem(OrderItem targetOrderItem, OrderItem orderItem)
    {
        targetOrderItem.Order = orderItem.Order;
        //targetOrderItem.OrderItem = orderItem;
        targetOrderItem.ItemId = orderItem!.ItemId;
        targetOrderItem.Quantity = orderItem.Quantity;
        targetOrderItem.Unit = orderItem.Unit;
        targetOrderItem.Price = orderItem.Price;
        targetOrderItem.Discount = orderItem.Discount;
        targetOrderItem.Total = orderItem.Total;
        targetOrderItem.Note = orderItem?.Note;
    }
}