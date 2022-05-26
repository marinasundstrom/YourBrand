using Documents.Domain.Common;

namespace Documents.Domain.Events;

public class DocumentRenamed : DomainEvent
{
    public DocumentRenamed(string documentId, string newName, string oldName)
    {
        DocumentId = documentId;
        NewName = newName;
        OldName = oldName;
    }

    public string DocumentId { get; }
    
    public string NewName { get; }

    public string OldName { get; }
}