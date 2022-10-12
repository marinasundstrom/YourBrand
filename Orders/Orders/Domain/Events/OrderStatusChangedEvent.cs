using YourBrand.Orders.Domain.Common;
using YourBrand.Orders.Domain.Entities;

namespace YourBrand.Orders.Domain.Events;

public record OrderStatusChangedEvent(int OrderNo, OrderStatus Status) : DomainEvent;
