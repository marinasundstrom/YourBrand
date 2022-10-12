using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Events;

public record OrderItemAddedEvent(int OrderNo, Guid OrderItemId) : DomainEvent;

