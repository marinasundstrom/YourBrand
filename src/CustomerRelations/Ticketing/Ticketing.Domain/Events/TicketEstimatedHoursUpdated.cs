using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketEstimatedHoursUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, double? NewHours, double? OldHours) : TicketDomainEvent(OrganizationId, TicketId);