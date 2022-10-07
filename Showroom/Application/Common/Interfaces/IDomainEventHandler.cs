using MediatR;

using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}