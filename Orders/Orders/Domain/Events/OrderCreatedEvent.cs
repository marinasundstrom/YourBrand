using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Events;

public record OrderCreatedEvent(int OrderNo) : DomainEvent;

