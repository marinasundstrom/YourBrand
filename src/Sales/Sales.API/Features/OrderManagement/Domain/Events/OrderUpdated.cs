using YourBrand.Domain;

namespace YourBrand.Sales.Features.OrderManagement.Domain.Events;

public sealed record OrderUpdated(string OrderId) : DomainEvent;