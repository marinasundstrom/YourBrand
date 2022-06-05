using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Domain.Events;

public class DocumentDeleted : DomainEvent
{
    public DocumentDeleted(string documentId)
    {
        DocumentId = documentId;
    }

    public string DocumentId { get; }
}
