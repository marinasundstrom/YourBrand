using YourBrand.Tenancy;
using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketAssigneeUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, TicketParticipantId? AssignedUserId, TicketParticipantId? OldAssignedUserId) : TicketDomainEvent(OrganizationId, TicketId);