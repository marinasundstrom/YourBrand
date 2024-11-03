
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

public class Team : AuditableEntity<string>, ISoftDeletable, IHasTenant, IHasOrganization
{
    readonly HashSet<User> _members = new HashSet<User>();
    readonly HashSet<TeamMembership> _memberships = new HashSet<TeamMembership>();
    readonly HashSet<Project> _projects = new HashSet<Project>();
    readonly HashSet<ProjectTeam> _projectTeams = new HashSet<ProjectTeam>();

    public Team(string id, string name, string? description) : base(id)
    {
        Name = name;
        Description = description;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Organization Organization { get; set; } = null!;

    public IReadOnlyCollection<User> Members => _members;

    public IReadOnlyCollection<TeamMembership> Memberships => _memberships;

    public void AddMember(User user)
    {
        _memberships.Add(new TeamMembership(user));
    }

    public void RemoveMember(User user)
    {
        _members.Remove(user);
    }

    public IReadOnlyCollection<Project> Projects => _projects;

    public IReadOnlyCollection<ProjectTeam> ProjectTeams => _projectTeams;

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}