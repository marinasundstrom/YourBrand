using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;
using YourBrand.Ticketing.Domain.Enums;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketUrgencyUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, TicketUrgency? NewUrgency, TicketUrgency? OldUrgency) : TicketDomainEvent(OrganizationId, TicketId);
