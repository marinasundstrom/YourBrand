using YourBrand.Documents.Application.Common.Interfaces;
using YourBrand.Documents.Domain.Events;

namespace YourBrand.Documents.Application.Events;

public class DocumentExpiredHandler : IDomainEventHandler<DocumentExpired>
{
    public Task Handle(DocumentExpired notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}