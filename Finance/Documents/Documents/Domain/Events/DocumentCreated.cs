using Documents.Domain.Common;

namespace Documents.Domain.Events;

public class DocumentCreated : DomainEvent
{
    public DocumentCreated(string documentId)
    {
        DocumentId = documentId;
    }

    public string DocumentId { get; }
}
