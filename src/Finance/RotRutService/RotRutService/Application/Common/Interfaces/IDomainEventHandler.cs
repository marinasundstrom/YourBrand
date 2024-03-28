using MediatR;

using YourBrand.RotRutService.Domain.Common;

namespace YourBrand.RotRutService.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}