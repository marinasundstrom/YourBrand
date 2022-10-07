using MediatR;

using YourBrand.Payments.Domain.Common;

namespace YourBrand.Payments.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}