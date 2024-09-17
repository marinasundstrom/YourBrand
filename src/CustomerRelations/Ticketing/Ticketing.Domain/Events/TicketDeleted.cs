using YourBrand.Tenancy;
using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketDeleted(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, string Title) : TicketDomainEvent(OrganizationId, TicketId);