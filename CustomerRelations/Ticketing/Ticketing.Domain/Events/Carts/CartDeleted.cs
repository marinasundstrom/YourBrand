namespace YourBrand.Ticketing.Domain.Events;

public sealed record IssueDeleted(string IssueId) : DomainEvent;