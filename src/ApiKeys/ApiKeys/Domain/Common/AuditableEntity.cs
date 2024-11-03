using YourBrand.ApiKeys.Domain.Entities;
using YourBrand.Auditability;
using YourBrand.Identity;

namespace YourBrand.ApiKeys.Domain.Common;

public abstract class AuditableEntity<TId> : Entity<TId>, IAuditableEntity<TId, User>
    where TId : notnull
{
    #nullable disable

    protected AuditableEntity() : base() {}

    #nullable restore

    public AuditableEntity(TId id) : base(id) 
    {

    }

    public DateTimeOffset Created { get; set; }

    public User? CreatedBy { get; set; } = null!;

    public UserId? CreatedById { get; set; } = null!;

    public DateTimeOffset? LastModified { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }
}