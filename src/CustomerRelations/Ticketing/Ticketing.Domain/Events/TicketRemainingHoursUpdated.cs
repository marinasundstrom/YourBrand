
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketRemainingHoursUpdated(TenantId TenantId, string OrganizationId, TicketId TicketId, double? hHurs, double? OldHours) : DomainEvent;