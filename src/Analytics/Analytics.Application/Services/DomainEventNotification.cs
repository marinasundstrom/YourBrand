using MediatR;

namespace YourBrand.Analytics.Application.Services;

public sealed class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent) : INotification where TDomainEvent : DomainEvent
{
    public TDomainEvent DomainEvent { get; } = domainEvent;
}