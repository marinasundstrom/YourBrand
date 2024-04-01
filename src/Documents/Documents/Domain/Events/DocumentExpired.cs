using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Domain.Events;

public record DocumentExpired : DomainEvent
{
    public DocumentExpired(string documentId)
    {
        DocumentId = documentId;
    }

    public string DocumentId { get; }
}