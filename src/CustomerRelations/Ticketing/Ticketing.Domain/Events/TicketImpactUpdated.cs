using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;
using YourBrand.Ticketing.Domain.Enums;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketImpactUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, TicketImpact? NewImpact, TicketImpact? OldImpact) : TicketDomainEvent(OrganizationId, TicketId);
