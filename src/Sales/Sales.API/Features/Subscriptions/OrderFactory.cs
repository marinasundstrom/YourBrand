using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Features.Orders;

public class OrderFactory
{
    public Order CreateOrder(Order order)
    {
        var targetOrder = Order.Create(order.OrganizationId);

        UpdateOrder(targetOrder, order);
        return targetOrder;
    }

    public Order CreateOrderFromOrderItem(OrderItem orderItem)
    {
        var targerOrder = Order.Create(orderItem.OrganizationId);

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

        if (order?.Customer is not null)
        {
            targetOrder.Customer = new Customer
            {
                Id = order.Customer.Id,
                Name = order.Customer.Name,
                CustomerNo = order.Customer.CustomerNo
            };
        }

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

        if (orderItem.Order?.Customer is not null)
        {
            targetOrder.Customer = new Customer
            {
                Id = orderItem.Order.Customer.Id,
                Name = orderItem.Order.Customer.Name,
                CustomerNo = orderItem.Order.Customer.CustomerNo
            };
        }

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

        targetOrderItem.SubscriptionPlan = orderItem!.SubscriptionPlan;
        targetOrderItem.Subscription = orderItem!.Subscription;
    }
}