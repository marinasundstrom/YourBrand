using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Events;

public record OrderCancelledEvent(int OrderNo) : DomainEvent;