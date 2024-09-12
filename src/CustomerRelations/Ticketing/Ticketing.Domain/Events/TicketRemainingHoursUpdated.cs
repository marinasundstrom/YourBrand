namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketRemainingHoursUpdated(string TenantId, string OrganizationId, int TicketId, double? hHurs, double? OldHours) : DomainEvent;