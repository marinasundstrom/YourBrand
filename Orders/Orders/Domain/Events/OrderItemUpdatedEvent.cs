using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Events;

public record OrderItemUpdatedEvent(int OrderNo, Guid OrderItemId) : DomainEvent;
