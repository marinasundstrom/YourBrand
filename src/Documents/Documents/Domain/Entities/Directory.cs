using YourBrand.Documents.Domain.Common;
using YourBrand.Documents.Domain.Events;
using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.Documents.Domain.Entities;

public class Directory : AuditableEntity<string>, ISoftDeletable, IDeletable, IItem
{
    private readonly HashSet<Document> _documents = new HashSet<Document>();
    private readonly HashSet<Directory> _directories = new HashSet<Directory>();

    private Directory()
    {
    }

    public Directory(string name) : base(Guid.NewGuid().ToString())
    {
        Name = name;

        AddDomainEvent(new DirectoryCreated(Id));
    }

    public Directory? Parent { get; private set; }

    public string? ParentId { get; private set; }

    Directory? IItem.Directory => Parent;

    public string Name { get; private set; } = null!;

    public bool Rename(string newName)
    {
        var oldName = Name;

        if (newName != oldName)
        {
            Name = newName;

            AddDomainEvent(new DirectoryRenamed(Id, newName, oldName));
            return true;
        }

        return false;
    }

    public string? Description { get; private set; } = null!;

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public IReadOnlyCollection<Document> Documents => _documents;

    public void AddDocument(Document document)
    {
        _documents.Add(document);
    }

    public IReadOnlyCollection<Directory> Directories => _directories;

    public Directory CreateDirectory(string name)
    {
        var directory = new Directory(name);
        _directories.Add(directory);
        return directory;
    }

    public DomainEvent GetDeleteEvent() => new DirectoryDeleted(Id, string.Empty);

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }
}