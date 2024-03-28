
using YourBrand.Documents.Application.Common.Models;
using YourBrand.Documents.Domain.Events;

using MediatR;
using YourBrand.Documents.Application.Common.Interfaces;

namespace YourBrand.Documents.Application.Events;

public class DocumentExpiredHandler : IDomainEventHandler<DocumentExpired>
{
    public Task Handle(DocumentExpired notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}