using YourBrand.Domain;

namespace YourBrand.Sales.Features.OrderManagement.Domain.Events;

public sealed record OrderCreated(string OrderId) : DomainEvent;

public sealed record ProductPriceListCreated(string ProductPriceListId) : DomainEvent;