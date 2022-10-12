using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Events;

public record OrderPlacedEvent(int OrderNo) : DomainEvent;
