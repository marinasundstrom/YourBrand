using YourBrand.Ticketing.Domain.Entities;

namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketStatusUpdated(int TicketId, TicketStatus NewStatus, TicketStatus OldStatus) : DomainEvent;