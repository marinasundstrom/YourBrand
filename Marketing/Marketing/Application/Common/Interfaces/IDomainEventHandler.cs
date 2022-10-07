using MediatR;

using YourBrand.Marketing.Domain.Common;

namespace YourBrand.Marketing.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}