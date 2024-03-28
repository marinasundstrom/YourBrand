namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketCreated(int TicketId) : DomainEvent;
