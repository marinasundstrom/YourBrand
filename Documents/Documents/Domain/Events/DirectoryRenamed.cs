using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Domain.Events;

public class DirectoryRenamed : DomainEvent
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