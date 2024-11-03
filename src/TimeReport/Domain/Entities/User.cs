
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

public class User : AuditableEntity<UserId>, ISoftDeletable, IHasTenant
{
    readonly HashSet<OrganizationUser> _organizationUsers = new HashSet<OrganizationUser>();
    readonly HashSet<TeamMembership> _teamMemberships = new HashSet<TeamMembership>();
    readonly HashSet<Organization> _organizations = new HashSet<Organization>();
    readonly HashSet<Team> _teams = new HashSet<Team>();

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

    public IReadOnlyCollection<Organization> Organizations => _organizations;

    public IReadOnlyCollection<Team> Teams => _teams;

    public IReadOnlyCollection<OrganizationUser> OrganizationUsers => _organizationUsers;

    public IReadOnlyCollection<TeamMembership> TeamMemberships => _teamMemberships;

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }

    public bool Hidden { get; set; }
}