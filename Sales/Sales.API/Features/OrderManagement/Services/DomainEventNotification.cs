using MediatR;

using YourBrand.Domain;
using YourBrand.Sales.API.Features.OrderManagement.Domain;

namespace YourBrand.Orders.Application.Services;

public sealed class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
{
    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }

    public TDomainEvent DomainEvent { get; }
}