using YourBrand.Identity;

namespace YourBrand.Inventory.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime Created { get; set; }

    public UserId CreatedById { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public UserId? LastModifiedById { get; set; }
}