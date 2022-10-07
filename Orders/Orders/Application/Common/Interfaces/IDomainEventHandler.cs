using MediatR;

using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}