using YourBrand.Documents.Domain.Common;
using YourBrand.Domain;

namespace YourBrand.Documents.Domain.Events;

public record DocumentDeleted : DomainEvent
{
    public DocumentDeleted(string documentId, string? cause)
    {
        DocumentId = documentId;
        Cause = cause;
    }

    public string DocumentId { get; }

    public string? Cause { get; }
}