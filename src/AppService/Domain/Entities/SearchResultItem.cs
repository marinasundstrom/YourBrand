using YourBrand.Domain.Common;
using YourBrand.Identity;

namespace YourBrand.Domain.Entities;

public class SearchResultItem : AuditableEntity<string>, ISoftDeletable
{
    protected SearchResultItem()
    {

    }

    public SearchResultItem(string name, string? description = null) : base(Guid.NewGuid().ToString())
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public string? Image { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}