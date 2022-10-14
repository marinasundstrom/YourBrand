using MediatR;

using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}