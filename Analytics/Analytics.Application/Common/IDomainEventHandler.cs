using System;
using MediatR;

namespace YourBrand.Analytics.Application.Common;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}