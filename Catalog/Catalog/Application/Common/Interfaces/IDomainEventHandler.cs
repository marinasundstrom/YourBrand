using MediatR;

using YourBrand.Catalog.Domain.Common;

namespace YourBrand.Catalog.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}