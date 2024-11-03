using MediatR;

using YourBrand.Domain;

namespace YourBrand.Analytics.Application.Common.Models;

public class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent) : INotification where TDomainEvent : DomainEvent
{
    public TDomainEvent DomainEvent { get; } = domainEvent;
}