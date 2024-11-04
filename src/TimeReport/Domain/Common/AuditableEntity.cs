using YourBrand.Auditability;
using YourBrand.Identity;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain.Common;

public abstract class AuditableEntity<TId> : Entity<TId>, IAuditableEntity<TId, User>
    where TId : notnull
{
#nullable disable

    protected AuditableEntity() : base() { }

#nullable restore

    public AuditableEntity(TId id) : base(id)
    {

    }

    public DateTimeOffset Created { get; set; }

    public UserId? CreatedById { get; set; }

    public User? CreatedBy { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public UserId? LastModifiedById { get; set; }

    public User? LastModifiedBy { get; set; } = null!;
}