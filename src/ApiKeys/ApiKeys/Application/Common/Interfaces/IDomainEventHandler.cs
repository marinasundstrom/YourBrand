using MediatR;

using YourBrand.ApiKeys.Domain.Common;

namespace YourBrand.ApiKeys.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}