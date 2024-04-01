
using YourBrand.Documents.Application.Common.Interfaces;
using YourBrand.Documents.Domain.Events;

namespace YourBrand.Documents.Application.Events;

public class DocumentCreatedHandler : IDomainEventHandler<DocumentCreated>
{
    public Task Handle(DocumentCreated notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}