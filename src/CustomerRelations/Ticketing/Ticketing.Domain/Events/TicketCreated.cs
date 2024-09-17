using YourBrand.Tenancy;
using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketCreated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId) : TicketDomainEvent(OrganizationId, TicketId);