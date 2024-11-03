using YourBrand.Documents.Domain.Common;
using YourBrand.Domain;

namespace YourBrand.Documents.Domain.Events;

public record DirectoryRenamed : DomainEvent
{
    public DirectoryRenamed(string documentId, string newName, string oldName)
    {
        DocumentId = documentId;
        NewName = newName;
        OldName = oldName;
    }

    public string DocumentId { get; }

    public string NewName { get; }

    public string OldName { get; }
}