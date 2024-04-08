using YourBrand.ApiKeys.Domain.Entities;
using YourBrand.Identity;

namespace YourBrand.ApiKeys.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTimeOffset Created { get; set; }

    public User? CreatedBy { get; set; } = null!;

    public UserId? CreatedById { get; set; } = null!;

    public DateTimeOffset? LastModified { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }
}