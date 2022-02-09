
using Catalog.Domain.Common;

namespace Catalog.Domain.Entities;

public class Comment : AuditableEntity, ISoftDelete, IHasDomainEvent
{
    protected Comment()
    {

    }

    public Comment(string text)
    {
        Id = Guid.NewGuid().ToString();
        Text = text;
    }

    public string Id { get; private set; } = null!;

    public Item Item { get; set; } = null!;

    public string Text { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}