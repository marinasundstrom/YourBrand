
using YourBrand.Domain.Common;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Domain.Entities;

public class User : AuditableEntity<UserId>, ISoftDeletable, IHasTenant
{
    public User() : base(Guid.NewGuid().ToString())
    {
    }

    public User(UserId id) : base(id)
    {
    }

    public TenantId TenantId { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? DisplayName { get; set; }

    public string SSN { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public bool Hidden { get; set; }
}