using YourBrand.Auditability;
using YourBrand.Identity;

namespace YourBrand.RotRutService.Domain.Common;

public abstract class AuditableEntity<TId> : Entity<TId>, IAuditableEntity<TId>
    where TId : notnull
{
#nullable disable

    protected AuditableEntity() : base() { }

#nullable restore

    public AuditableEntity(TId id) : base(id)
    {

    }

    public DateTimeOffset Created { get; set; }

    public UserId? CreatedById { get; set; } = null!;

    public DateTimeOffset? LastModified { get; set; }

    public UserId? LastModifiedById { get; set; }
}