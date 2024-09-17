using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketRemainingHoursUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, double? hHurs, double? OldHours) : TicketDomainEvent(OrganizationId, TicketId);