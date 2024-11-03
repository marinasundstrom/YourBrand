﻿using YourBrand.Documents.Application.Common.Interfaces;
using YourBrand.Documents.Domain.Events;
using YourBrand.Domain;

namespace YourBrand.Documents.Application.Events;

public class DocumentRenamedHandler : IDomainEventHandler<DocumentRenamed>
{
    public Task Handle(DocumentRenamed notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}