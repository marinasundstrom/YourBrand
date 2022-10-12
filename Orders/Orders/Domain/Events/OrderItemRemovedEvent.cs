using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Events;

public record OrderItemRemovedEvent(int OrderNo, Guid OrderItemId) : DomainEvent;
