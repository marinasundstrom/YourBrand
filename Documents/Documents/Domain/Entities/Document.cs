using YourBrand.Documents.Domain.Common;
using YourBrand.Documents.Domain.Events;

namespace YourBrand.Documents.Domain.Entities;

public class Document : AuditableEntity, ISoftDelete, IHasDomainEvents, IDeletable
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

    public bool Rename(string newName)
    {
        var oldName = Name;

        if (newName != oldName)
        {
            Name = newName;

            DomainEvents.Add(new DocumentRenamed(Id, newName, oldName));
            return true;
        }

        return false;
    }

    public string Extension { get; private set; } = null!;

    public string ContentType { get; private set; } = null!;

    //public DocumentType DocumentType { get; private set; }

    public string BlobId { get; set; } = null!;

    public string? Description { get; private set; } = null!;

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public DateTime? Expiration { get; private set; }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

    public DomainEvent GetDeleteEvent() => new DocumentDeleted(Id, string.Empty);

    public DateTime? Deleted { get; set; }

    public string? DeletedById { get; set; }
}