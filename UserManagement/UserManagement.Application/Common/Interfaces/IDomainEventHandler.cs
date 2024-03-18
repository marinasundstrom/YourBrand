using MediatR;

using YourBrand.UserManagement.Domain.Common;

namespace YourBrand.UserManagement.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}