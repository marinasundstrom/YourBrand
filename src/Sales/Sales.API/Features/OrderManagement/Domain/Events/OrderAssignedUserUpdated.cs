using YourBrand.Domain;

namespace YourBrand.Sales.Features.OrderManagement.Domain.Events;

public sealed record OrderAssignedUserUpdated(string OrderId, string? AssignedUserId, string? OldAssignedUserId) : DomainEvent;