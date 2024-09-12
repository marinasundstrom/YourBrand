using YourBrand.Tenancy;
using YourBrand.Domain;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketAssigneeUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, TicketParticipantId? AssignedUserId, TicketParticipantId? OldAssignedUserId) : DomainEvent;