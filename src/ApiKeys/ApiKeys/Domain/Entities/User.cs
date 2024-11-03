
using YourBrand.ApiKeys.Domain.Common;
using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.ApiKeys.Domain.Entities;

public class User : AuditableEntity<UserId>, ISoftDeletableWithAudit<User>
{
    protected User()
    {
    }

    public User(UserId id) : base(id)
    {
    }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? DisplayName { get; set; }

    public string Email { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public bool Hidden { get; set; }
}