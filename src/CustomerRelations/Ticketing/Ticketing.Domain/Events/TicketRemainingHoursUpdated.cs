namespace YourBrand.Ticketing.Domain.Events;

public sealed record TicketRemainingHoursUpdated(int TicketId, double? hHurs, double? OldHours) : DomainEvent;