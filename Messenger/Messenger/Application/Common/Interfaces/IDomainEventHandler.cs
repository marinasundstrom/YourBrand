using MediatR;

using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}