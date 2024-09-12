namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketSubjectUpdated(string TenantId, string OrganizationId, int TicketId, string Title) : DomainEvent;