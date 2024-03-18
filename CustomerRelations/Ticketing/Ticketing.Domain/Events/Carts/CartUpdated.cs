namespace YourBrand.Ticketing.Domain.Events;

public sealed record IssueUpdated(string IssueId) : DomainEvent;