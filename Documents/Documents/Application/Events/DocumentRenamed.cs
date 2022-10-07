
using YourBrand.Documents.Application.Common.Models;
using YourBrand.Documents.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;
using YourBrand.Documents.Application.Common.Interfaces;

namespace YourBrand.Documents.Application.Events;

public class DocumentRenamedHandler : IDomainEventHandler<DocumentRenamed>
{
    public Task Handle(DocumentRenamed notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
