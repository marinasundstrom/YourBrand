using YourBrand.Domain;

namespace YourBrand.Sales.Domain.Events;
public sealed record OrderStatusUpdated(string OrderId, int NewStatus, int OldStatus) : DomainEvent;