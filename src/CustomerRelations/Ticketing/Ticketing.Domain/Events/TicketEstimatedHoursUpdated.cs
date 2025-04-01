using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

using OrganizationId = YourBrand.Domain.OrganizationId;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketEstimatedTimeUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, TimeSpan? NewTime, TimeSpan? OldTime) : TicketDomainEvent(OrganizationId, TicketId);