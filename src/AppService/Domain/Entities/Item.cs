using YourBrand.Domain.Common;

namespace YourBrand.Domain.Entities;

public class Item : AuditableEntity, ISoftDelete
{
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

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}