
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class User : AuditableEntity, ISoftDelete
{
    readonly List<OrganizationUser> _organizationUsers = new List<OrganizationUser>();
    readonly List<TeamMembership> _teamMemberships = new List<TeamMembership>();

    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? DisplayName { get; set; }

    public string SSN { get; set; } = null!;

    public string Email { get; set; } = null!;

    public List<Organization> Organizations { get; set; } = new List<Organization>();

    public List<Team> Teams { get; set; } = new List<Team>();

    public IReadOnlyCollection<OrganizationUser> OrganizationUsers => _organizationUsers.AsReadOnly();

    public IReadOnlyCollection<TeamMembership> TeamMemberships => _teamMemberships.AsReadOnly();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public bool Hidden { get; set; }
}