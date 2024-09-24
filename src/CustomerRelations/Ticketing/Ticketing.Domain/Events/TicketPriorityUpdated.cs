using OrganizationId = YourBrand.Domain.OrganizationId;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;
using YourBrand.Ticketing.Domain.Enums;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketPriorityUpdated(TenantId TenantId, OrganizationId OrganizationId, TicketId TicketId, TicketPriority? NewPriority, TicketPriority? OldPriority) : TicketDomainEvent(OrganizationId, TicketId);
