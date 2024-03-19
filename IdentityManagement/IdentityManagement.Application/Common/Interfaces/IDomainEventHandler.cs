using MediatR;

using YourBrand.IdentityManagement.Domain.Common;

namespace YourBrand.IdentityManagement.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}