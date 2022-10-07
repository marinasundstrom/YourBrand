using MediatR;

using YourBrand.Warehouse.Domain.Common;

namespace YourBrand.Warehouse.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}