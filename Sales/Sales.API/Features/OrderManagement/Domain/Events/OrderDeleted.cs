using YourBrand.Domain;

namespace YourBrand.Sales.API.Features.OrderManagement.Domain.Events;

public sealed record OrderDeleted(int OrderNo) : DomainEvent;