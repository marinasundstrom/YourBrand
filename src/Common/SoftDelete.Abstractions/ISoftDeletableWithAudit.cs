using YourBrand.Identity;

namespace YourBrand.Domain;

public interface ISoftDeletableWithAudit : ISoftDeletable
{
    DateTimeOffset? Deleted { get; set; }
    UserId? DeletedById { get; set; }
}

public interface ISoftDeletableWithAudit<TUser> : ISoftDeletableWithAudit
    where TUser : class, IEntity<UserId>
{
    TUser? DeletedBy { get; set; }
}