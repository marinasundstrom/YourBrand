using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketAssigneeUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, TicketParticipantId? NewAssignedParticipantId, TicketParticipantId? OldAssignedParticipantId) : TicketDomainEvent(OrganizationId, TicketId);