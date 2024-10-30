using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Features.Orders;

public class OrderFactory(TimeProvider timeProvider)
{
    public Order CreateOrder(Order order)
    {
        var targetOrder = new Order();
        targetOrder.OrganizationId = order.OrganizationId;

        order.CopyTo(targetOrder, timeProvider, true, true);
        return targetOrder;
    }

    public Order CreateOrderFromOrderItem(OrderItem orderItem)
    {
        var targetOrder = new Order();
        targetOrder.OrganizationId = orderItem.OrganizationId;

        orderItem.CopyToOrder(targetOrder, timeProvider);
        return targetOrder;
    }

    public OrderItem CreateOrderItem(OrderItem orderItem)
    {
        var targetOrderItem = new OrderItem();
        targetOrderItem.OrganizationId = orderItem.OrganizationId;

        orderItem.CopyTo(targetOrderItem);
        return targetOrderItem;
    }
}