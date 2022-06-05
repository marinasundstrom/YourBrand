
using YourBrand.Documents.Application.Common.Models;
using YourBrand.Documents.Domain.Events;

using MediatR;

namespace YourBrand.Documents.Application.Events;

public class DocumentCreatedHandler : INotificationHandler<DomainEventNotification<DocumentCreated>>
{
    public Task Handle(DomainEventNotification<DocumentCreated> notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
