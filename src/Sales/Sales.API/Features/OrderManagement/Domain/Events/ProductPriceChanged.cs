using YourBrand.Domain;

namespace YourBrand.Sales.Features.OrderManagement.Domain.Events;

public sealed record ProductPriceChanged(string ProductId, decimal Price, decimal? RegularPrice) : DomainEvent;