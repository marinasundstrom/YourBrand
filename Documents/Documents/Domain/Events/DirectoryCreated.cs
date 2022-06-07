using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Domain.Events;

public class DirectoryCreated : DomainEvent
{
    public DirectoryCreated(string documentId)
    {
        DocumentId = documentId;
    }

    public string DocumentId { get; }
}
