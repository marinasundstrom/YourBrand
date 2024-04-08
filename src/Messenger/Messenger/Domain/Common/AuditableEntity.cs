using YourBrand.Identity;
using YourBrand.Messenger.Domain.Entities;

namespace YourBrand.Messenger.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime Created { get; set; }

    public User? CreatedBy { get; set; } = null!;

    public UserId? CreatedById { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }
}