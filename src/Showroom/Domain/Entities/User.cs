
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class User : AuditableEntity<UserId>, IHasTenant, ISoftDeletableWithAudit<User>
{
    readonly HashSet<OrganizationUser> _organizationUsers = new HashSet<OrganizationUser>();
    readonly HashSet<Organization> _organization = new HashSet<Organization>();

    public User()
        : base(Guid.NewGuid().ToString())
    {

    }

    public User(UserId id) : base(id)
    {

    }

    public TenantId TenantId { get; set; } = null!;

    public OrganizationId? OrganizationId { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? DisplayName { get; set; }

    public string SSN { get; set; } = null!;

    public string Email { get; set; } = null!;

    public IReadOnlyCollection<Organization> Organization => _organization;
    public IReadOnlyCollection<OrganizationUser> OrganizationUsers => _organizationUsers;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public bool Hidden { get; set; }
}