using MediatR;

using YourBrand.Domain;
using YourBrand.Sales.Features.OrderManagement.Domain;

namespace YourBrand.Sales.Features.Services;

public sealed class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
{
    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }

    public TDomainEvent DomainEvent { get; }
}