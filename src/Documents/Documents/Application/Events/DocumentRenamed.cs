using YourBrand.Documents.Application.Common.Interfaces;
using YourBrand.Documents.Domain.Events;

namespace YourBrand.Documents.Application.Events;

public class DocumentRenamedHandler : IDomainEventHandler<DocumentRenamed>
{
    public Task Handle(DocumentRenamed notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}