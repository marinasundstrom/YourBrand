using YourBrand.Domain.Common;
using YourBrand.Identity;

namespace YourBrand.Domain.Entities;

public class SearchResultItem : AuditableEntity, ISoftDeletable
{
    protected SearchResultItem()
    {

    }

    public SearchResultItem(string name, string? description = null)
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
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}