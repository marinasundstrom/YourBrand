namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketTextUpdated(string TenantId, string OrganizationId, int TicketId, string? Description) : DomainEvent;