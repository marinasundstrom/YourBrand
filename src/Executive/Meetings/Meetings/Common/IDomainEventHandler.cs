using MediatR;

namespace YourBrand.Meetings.Common;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}