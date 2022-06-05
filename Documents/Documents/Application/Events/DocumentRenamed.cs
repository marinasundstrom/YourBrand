
using YourBrand.Documents.Application.Common.Models;
using YourBrand.Documents.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

namespace YourBrand.Documents.Application.Events;

public class DocumentRenamedHandler : INotificationHandler<DomainEventNotification<DocumentRenamed>>
{
    public Task Handle(DomainEventNotification<DocumentRenamed> notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
