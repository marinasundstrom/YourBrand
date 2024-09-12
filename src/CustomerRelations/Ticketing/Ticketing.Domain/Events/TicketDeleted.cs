namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketDeleted(string TenantId, string OrganizationId, int TicketId, string Title) : DomainEvent;