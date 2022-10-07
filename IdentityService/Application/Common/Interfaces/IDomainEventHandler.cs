using MediatR;

using YourBrand.IdentityService.Domain.Common;

namespace YourBrand.IdentityService.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}