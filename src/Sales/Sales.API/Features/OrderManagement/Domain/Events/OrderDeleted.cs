using YourBrand.Domain;

namespace YourBrand.Sales.Features.OrderManagement.Domain.Events;

public sealed record OrderDeleted(int OrderNo) : DomainEvent;