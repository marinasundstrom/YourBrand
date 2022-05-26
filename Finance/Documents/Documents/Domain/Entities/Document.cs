using Documents.Domain.Common;
using Documents.Domain.Events;

namespace Documents.Domain.Entities;

public class Document : AuditableEntity, IHasDomainEvents
{
    private Document() 
    {
    }

    public Document(string name, string contentType) 
    {
        Id = Guid.NewGuid().ToString();
        Name = Path.GetFileNameWithoutExtension(name);
        Extension = Path.GetExtension(name).Trim('.');
        ContentType = contentType;
        
        DomainEvents.Add(new DocumentCreated(Id));
    }

    public string Id { get; private set; } = null!;

    public string Name { get; private set; } = null!;

    public string Extension { get; private set; } = null!;

    public string ContentType { get; private set; } = null!;

    public string BlobId { get; set; } = null!;

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

    public bool Rename(string newName)
    {
        var oldName = Name;

        if(newName != oldName) 
        {
            Name = newName;

            DomainEvents.Add(new DocumentRenamed(Id, newName, oldName));
            return true;
        }

        return false;
    }
}