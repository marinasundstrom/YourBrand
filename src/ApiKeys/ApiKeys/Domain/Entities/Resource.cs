using YourBrand.ApiKeys.Domain.Common;
using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.ApiKeys.Domain.Entities;

public class Resource : AuditableEntity<string>, ISoftDeletableWithAudit<User>
{
    protected Resource()
    {
    }

    public Resource(string name, string? description) : base(Guid.NewGuid().ToString())
    {
        Name = name;
        Description = description;
    }

    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public Service Service { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}