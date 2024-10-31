using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.Auditability;

public interface IAuditableEntity : IEntity
{
    UserId? CreatedById { get; set; }
    DateTimeOffset Created { get; set; }

    UserId? LastModifiedById { get; set; }
    DateTimeOffset? LastModified { get; set; }
}

public interface IAuditableEntity<TId> : IAuditableEntity, IEntity<TId>
{

}

public interface IAuditableEntity<TId, TUser> : IAuditableEntity<TId>
    where TUser : IEntity<UserId>
{
    TUser? CreatedBy { get; set; }
    TUser? LastModifiedBy { get; set; }
}