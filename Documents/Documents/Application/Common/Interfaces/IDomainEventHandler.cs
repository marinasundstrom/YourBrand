using MediatR;

using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Application.Common.Interfaces;

public interface IDomainEventHandler<TDomainEvent>
    : INotificationHandler<TDomainEvent>
    where TDomainEvent : DomainEvent
{

}