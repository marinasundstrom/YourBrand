using YourBrand.Documents.Application.Common.Interfaces;
using YourBrand.Documents.Domain.Events;
using YourBrand.Domain;

namespace YourBrand.Documents.Application.Events;

public class DocumentDeletedHandler : IDomainEventHandler<DocumentDeleted>
{
    public Task Handle(DocumentDeleted notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}