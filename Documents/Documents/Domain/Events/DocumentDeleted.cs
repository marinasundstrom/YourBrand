using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Domain.Events;

public class DocumentDeleted : DomainEvent
{
    public DocumentDeleted(string documentId, string? cause)
    {
        DocumentId = documentId;
        Cause = cause;
    }

    public string DocumentId { get; }

    public string? Cause { get; }
}