
using YourBrand.Domain.Common;

namespace YourBrand.Domain.Entities;

public class Comment : AuditableEntity, ISoftDelete
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
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}