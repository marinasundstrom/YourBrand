using YourBrand.Orders.Domain.Common;
using YourBrand.Orders.Domain.Entities;

namespace YourBrand.Orders.Domain.Events;

public class OrderCreatedEvent : DomainEvent
{
    public OrderCreatedEvent(int orderNo)
    {
        OrderNo = orderNo;
    }

    public int OrderNo { get; }
}

public class OrderItemAddedEvent : DomainEvent
{
    public OrderItemAddedEvent()
    {
    }

    public OrderItemAddedEvent(int orderNo, Guid orderItemId)
    {
        OrderNo = orderNo;
        OrderItemId = orderItemId;
    }

    public int OrderNo { get; set; }

    public Guid OrderItemId { get; set; }
}

public class OrderClearedEvent : DomainEvent
{
    public OrderClearedEvent(int orderNo)
    {
        OrderNo = orderNo;
    }

    public int OrderNo { get; }
}

public class OrderItemUpdatedEvent : DomainEvent
{
    public OrderItemUpdatedEvent(int orderNo, Guid orderItemId)
    {
        OrderNo = orderNo;
        OrderItemId = orderItemId;
    }

    public int OrderNo { get; }

    public Guid OrderItemId { get; }
}

public class OrderItemRemovedEvent : DomainEvent
{
    public OrderItemRemovedEvent(int orderNo, Guid orderItemId)
    {
        OrderNo = orderNo;
        OrderItemId = orderItemId;
    }

    public int OrderNo { get; }

    public Guid OrderItemId { get; }
}

public class OrderItemQuantityUpdatedEvent : DomainEvent
{
    public OrderItemQuantityUpdatedEvent(int orderNo, Guid orderItemId, double oldQuantity, double newQuantity)
    {
        OrderNo = orderNo;
        OrderItemId = orderItemId;
        OldQuantity = oldQuantity;
        NewQuantity = newQuantity;
    }

    public int OrderNo { get; }

    public Guid OrderItemId { get; set; }

    public double OldQuantity { get; set; }

    public double NewQuantity { get; set; }
}

public class OrderStatusChangedEvent : DomainEvent
{
    public OrderStatusChangedEvent(int orderNo, OrderStatus status)
    {
        OrderNo = orderNo;
        Status = status;
    }

    public int OrderNo { get; }

    public OrderStatus Status { get; }
}

public class OrderPlacedEvent : DomainEvent
{
    public OrderPlacedEvent(int orderNo)
    {
        OrderNo = orderNo;
    }

    public int OrderNo { get; }
}

public class OrderCancelledEvent : DomainEvent
{
    public OrderCancelledEvent(int orderNo)
    {
        OrderNo = orderNo;
    }

    public int OrderNo { get; }
}