using MediatR;

using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}