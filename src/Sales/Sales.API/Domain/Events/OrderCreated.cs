using YourBrand.Domain;

namespace YourBrand.Sales.Domain.Events;

public sealed record OrderCreated(string OrderId) : DomainEvent;

public sealed record ProductPriceListCreated(string ProductPriceListId) : DomainEvent;