namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketCreated(string TenantId, string OrganizationId, int TicketId) : DomainEvent;