using System;

using Catalog.Domain.Common;
using Catalog.Domain.Events;

namespace Catalog.Domain.Entities;

public class Item : AuditableEntity, ISoftDelete, IHasDomainEvent
{
    readonly List<Comment> _comments = new List<Comment>();

    protected Item()
    {

    }

    public Item(string name, string? description = null)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Description = description;
    }

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public string? Image { get; set; } = null!;

    public int CommentCount { get; set; }

    public IReadOnlyList<Comment> Comments => _comments.AsReadOnly();

    public void AddComment(string text)
    {
        var comment = new Comment(text);
        comment.DomainEvents.Add(new CommentPostedEvent(Id, comment.Id));
        _comments.Add(comment);
    }

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}