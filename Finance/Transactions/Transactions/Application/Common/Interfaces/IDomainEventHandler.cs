using MediatR;

using YourBrand.Transactions.Domain.Common;

namespace YourBrand.Transactions.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}