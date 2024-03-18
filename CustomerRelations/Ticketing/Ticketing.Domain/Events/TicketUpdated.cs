namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketUpdated(int TicketId) : DomainEvent;