using YourBrand.Ticketing.Domain.Entities;
using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketStatusUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, TicketStatus NewStatus, TicketStatus OldStatus) : TicketDomainEvent(OrganizationId, TicketId);