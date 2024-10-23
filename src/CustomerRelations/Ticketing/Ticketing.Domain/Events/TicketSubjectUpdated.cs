using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketSubjectUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, string NewSubject, string OldSubject) : TicketDomainEvent(OrganizationId, TicketId);