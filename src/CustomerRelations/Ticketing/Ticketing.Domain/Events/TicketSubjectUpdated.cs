using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketSubjectUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, string Title) : TicketDomainEvent(OrganizationId, TicketId);
