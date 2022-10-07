
using YourBrand.Documents.Application.Common.Models;
using YourBrand.Documents.Domain.Events;

using MediatR;
using YourBrand.Documents.Application.Common.Interfaces;

namespace YourBrand.Documents.Application.Events;

public class DocumentDeletedHandler : IDomainEventHandler<DocumentDeleted>
{
    public Task Handle(DocumentDeleted notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
