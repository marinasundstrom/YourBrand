using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId) : TicketDomainEvent(OrganizationId, TicketId);