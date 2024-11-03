using YourBrand.Documents.Domain.Common;
using YourBrand.Documents.Domain.Events;
using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.Documents.Domain.Entities;

public class Document : AuditableEntity<string>, ISoftDeletable, IDeletable, IItem
{
    private Document()
    {
    }

    public Document(string name, string contentType) : base(Guid.NewGuid().ToString())
    {
        Name = Path.GetFileNameWithoutExtension(name);
        Extension = Path.GetExtension(name).Trim('.');
        ContentType = contentType;

        AddDomainEvent(new DocumentCreated(Id));
    }

    public Directory? Directory { get; private set; }

    public string? DirectoryId { get; private set; }

    public string Name { get; private set; } = null!;

    public bool Rename(string newName)
    {
        var oldName = Name;

        if (newName != oldName)
        {
            Name = newName;

            AddDomainEvent(new DocumentRenamed(Id, newName, oldName));
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

    public DomainEvent GetDeleteEvent() => new DocumentDeleted(Id, string.Empty);

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }
}