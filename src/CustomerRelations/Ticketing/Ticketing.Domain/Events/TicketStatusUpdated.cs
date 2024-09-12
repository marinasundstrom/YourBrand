using YourBrand.Ticketing.Domain.Entities;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketStatusUpdated(string TenantId, string OrganizationId, int TicketId, TicketStatus NewStatus, TicketStatus OldStatus) : DomainEvent;