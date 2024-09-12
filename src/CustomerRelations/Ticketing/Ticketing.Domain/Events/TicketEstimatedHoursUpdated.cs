namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketEstimatedHoursUpdated(string TenantId, string OrganizationId, int TicketId, double? Hours, double? OldHours) : DomainEvent;