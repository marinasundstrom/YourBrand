using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketProjectUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, ProjectId NewProjectId, ProjectId OldProjectId) : TicketDomainEvent(OrganizationId, TicketId);
