
using YourBrand.Documents.Application.Common.Models;
using YourBrand.Documents.Domain.Events;

using MediatR;

namespace YourBrand.Documents.Application.Events;

public class DocumentExpiredHandler : INotificationHandler<DomainEventNotification<DocumentExpired>>
{
    public Task Handle(DomainEventNotification<DocumentExpired> notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}