using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.Enums;
using YourBrand.Ticketing.Domain.ValueObjects;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketImpactUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, TicketImpact? NewImpact, TicketImpact? OldImpact) : TicketDomainEvent(OrganizationId, TicketId);