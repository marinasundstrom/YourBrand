using YourBrand.Tenancy;
using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketCommentAdded(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, int CommentId) : TicketDomainEvent(OrganizationId, TicketId);