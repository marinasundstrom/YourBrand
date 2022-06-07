using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Domain.Events;

public class DirectoryDeleted : DomainEvent
{
    public DirectoryDeleted(string documentId, string? cause)
    {
        DocumentId = documentId;
        Cause = cause;
    }

    public string DocumentId { get; }

    public string? Cause { get; }
}