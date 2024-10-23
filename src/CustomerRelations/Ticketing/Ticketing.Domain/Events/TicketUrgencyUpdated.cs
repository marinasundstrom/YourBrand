using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.Enums;
using YourBrand.Ticketing.Domain.ValueObjects;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketUrgencyUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, TicketUrgency? NewUrgency, TicketUrgency? OldUrgency) : TicketDomainEvent(OrganizationId, TicketId);