using MediatR;

using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}