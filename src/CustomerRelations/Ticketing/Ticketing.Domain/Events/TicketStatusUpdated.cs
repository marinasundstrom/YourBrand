using YourBrand.Ticketing.Domain.Entities;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketStatusUpdated(TenantId TenantId, string OrganizationId, TicketId TicketId, TicketStatus NewStatus, TicketStatus OldStatus) : DomainEvent;