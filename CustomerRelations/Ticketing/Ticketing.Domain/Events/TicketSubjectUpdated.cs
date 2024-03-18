namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketSubjectUpdated(int TicketId, string Title) : DomainEvent;