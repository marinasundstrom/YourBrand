using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketEstimatedHoursUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, double? NewHours, double? OldHours) : TicketDomainEvent(OrganizationId, TicketId);