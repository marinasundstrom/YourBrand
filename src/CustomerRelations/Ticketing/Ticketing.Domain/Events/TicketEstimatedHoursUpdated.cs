using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketEstimatedHoursUpdated(TenantId TenantId, string OrganizationId, TicketId TicketId, double? Hours, double? OldHours) : DomainEvent;