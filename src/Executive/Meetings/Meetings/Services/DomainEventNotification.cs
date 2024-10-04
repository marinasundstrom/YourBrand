using MediatR;

namespace YourBrand.Meetings.Services;

public sealed class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent) : INotification where TDomainEvent : DomainEvent
{
    public TDomainEvent DomainEvent { get; } = domainEvent;
}