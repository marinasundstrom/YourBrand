using System;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Features.Orders;

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
        targetOrder.UpdateStatus(1);
        targetOrder.StatusDate = DateTimeOffset.UtcNow;

        targetOrder.Parent = order;
        targetOrder.Subscription = order.Subscription;
        targetOrder.Notes = order?.Notes;
        targetOrder.BillingDetails = order?.BillingDetails?.Copy();
        targetOrder.ShippingDetails = order?.ShippingDetails?.Copy();
        //delivery.Assignee = order?.Assignee;
    }

    public void UpdateOrder(Order targetOrder, OrderItem orderItem)
    {
        targetOrder.UpdateStatus(1);
        targetOrder.StatusDate = DateTimeOffset.UtcNow;

        targetOrder.Parent = orderItem.Order;
        targetOrder.Subscription = orderItem.Subscription ?? orderItem.Order.Subscription;
        targetOrder.BillingDetails = orderItem?.Order?.BillingDetails?.Copy(); // orderItem.HasDeliveryDetails ? orderItem?.DeliveryDetails?.Clone() : orderItem?.Order?.DeliveryDetails?.Clone();
        targetOrder.ShippingDetails = orderItem?.Order?.ShippingDetails?.Copy(); // orderItem.HasDeliveryDetails ? orderItem?.DeliveryDetails?.Clone() : orderItem?.Order?.DeliveryDetails?.Clone();
        //delivery.Assignee = orderItem?.Assignee ?? orderItem?.Order?.Assignee;
        targetOrder.Notes = orderItem!.Notes;
    }

    public void UpdateOrderItem(OrderItem targetOrderItem, OrderItem orderItem)
    {
        //targetOrderItem.Order = orderItem.Order;
        //targetOrderItem.OrderItem = orderItem;

        targetOrderItem.Description = orderItem.Description;
        targetOrderItem.ProductId = orderItem!.ProductId;
        targetOrderItem.Sku = orderItem!.Sku;
        targetOrderItem.Quantity = orderItem.Quantity;
        targetOrderItem.Unit = orderItem.Unit;
        targetOrderItem.Price = orderItem.Price;
        targetOrderItem.RegularPrice = orderItem.RegularPrice;
        targetOrderItem.VatRate = orderItem.VatRate;
        targetOrderItem.Discount = orderItem.Discount;
        targetOrderItem.DiscountRate = orderItem.DiscountRate;
        targetOrderItem.Notes = orderItem?.Notes;
    }
}