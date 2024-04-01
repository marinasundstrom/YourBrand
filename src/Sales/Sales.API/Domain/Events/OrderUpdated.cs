using YourBrand.Domain;

namespace YourBrand.Sales.Domain.Events;

public sealed record OrderUpdated(string OrderId) : DomainEvent;