using MediatR;

using YourBrand.Customers.Domain.Common;

namespace YourBrand.Customers.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}