using YourBrand.Domain;
using YourBrand.Sales.Features.OrderManagement.Domain.Entities;

namespace YourBrand.Sales.Features.OrderManagement.Domain.Events;
public sealed record OrderStatusUpdated(string OrderId, int NewStatus, int OldStatus) : DomainEvent;