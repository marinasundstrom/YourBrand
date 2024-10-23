using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketCommentAdded(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, int CommentId) : TicketDomainEvent(OrganizationId, TicketId);