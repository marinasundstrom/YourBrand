using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketDeleted(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, string Title) : TicketDomainEvent(OrganizationId, TicketId);