using MediatR;

using YourBrand.Notifications.Domain.Common;

namespace YourBrand.Notifications.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}