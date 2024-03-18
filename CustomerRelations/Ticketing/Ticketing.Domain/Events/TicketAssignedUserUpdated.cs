namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketAssignedUserUpdated(int TicketId, string? AssignedUserId, string? OldAssignedUserId) : DomainEvent;