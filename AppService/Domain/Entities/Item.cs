using System;

using YourBrand.Domain.Common;
using YourBrand.Domain.Events;

namespace YourBrand.Domain.Entities;

public class Item : AuditableEntity, ISoftDelete
{
    readonly HashSet<Comment> _comments = new HashSet<Comment>();

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

    public IReadOnlyCollection<Comment> Comments => _comments;

    public void AddComment(string text)
    {
        var comment = new Comment(text);
        comment.AddDomainEvent(new CommentPostedEvent(Id, comment.Id));
        _comments.Add(comment);
    }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}