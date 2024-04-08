using YourBrand.Identity;

namespace YourBrand.Accounting.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime Created { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTime? LastModified { get; set; }

    public UserId? LastModifiedById { get; set; }
}