namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketAssignedUserUpdated(string TenantId, string OrganizationId, int TicketId, string? AssignedUserId, string? OldAssignedUserId) : DomainEvent;