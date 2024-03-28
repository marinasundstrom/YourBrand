using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Domain.Events;

public record DirectoryCreated : DomainEvent
{
    public DirectoryCreated(string documentId)
    {
        DocumentId = documentId;
    }

    public string DocumentId { get; }
}
