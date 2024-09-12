namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketUpdated(string TenantId, string OrganizationId, int TicketId) : DomainEvent;