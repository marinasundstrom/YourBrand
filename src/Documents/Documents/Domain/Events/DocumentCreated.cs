using YourBrand.Documents.Domain.Common;
using YourBrand.Domain;

namespace YourBrand.Documents.Domain.Events;

public record DocumentCreated : DomainEvent
{
    public DocumentCreated(string documentId)
    {
        DocumentId = documentId;
    }

    public string DocumentId { get; }
}