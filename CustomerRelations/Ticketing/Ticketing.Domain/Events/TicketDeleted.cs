namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketDeleted(int TicketId, string Title) : DomainEvent;