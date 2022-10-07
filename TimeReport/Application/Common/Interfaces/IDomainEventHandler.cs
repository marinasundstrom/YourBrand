using MediatR;

using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}