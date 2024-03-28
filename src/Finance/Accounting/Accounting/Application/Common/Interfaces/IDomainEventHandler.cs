using MediatR;

using YourBrand.Accounting.Domain.Common;

namespace YourBrand.Accounting.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}