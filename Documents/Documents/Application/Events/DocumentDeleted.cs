
using YourBrand.Documents.Application.Common.Models;
using YourBrand.Documents.Domain.Events;

using MediatR;

namespace YourBrand.Documents.Application.Events;

public class DocumentDeletedHandler : INotificationHandler<DomainEventNotification<DocumentDeleted>>
{
    public Task Handle(DomainEventNotification<DocumentDeleted> notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
