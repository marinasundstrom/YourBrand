namespace YourBrand.Ticketing.Domain.Events;

public sealed record IssueCreated(string IssueId) : DomainEvent;
