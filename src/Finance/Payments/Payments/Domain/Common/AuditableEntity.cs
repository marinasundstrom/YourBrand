using YourBrand.Identity;

namespace YourBrand.Payments.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTimeOffset Created { get; set; }

    public UserId? CreatedById { get; set; } = null!;

    public DateTimeOffset? LastModified { get; set; }

    public UserId? LastModifiedById { get; set; }
}