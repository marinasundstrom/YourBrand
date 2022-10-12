using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Events;

public record OrderItemQuantityUpdatedEvent(int OrderNo, Guid OrderItemId, double OldQuantity, double NewQuantity) : DomainEvent;
