
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class Team : AuditableEntity, ISoftDelete, IHasTenant
{
    readonly HashSet<User> _members = new HashSet<User>();
    readonly HashSet<TeamMembership> _memberships = new HashSet<TeamMembership>();
    readonly HashSet<Project> _projects = new HashSet<Project>();
    readonly HashSet<ProjectTeam> _projectTeams = new HashSet<ProjectTeam>();

    protected Team()
    {

    }

    public Team(string id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Organization Organization { get; set; } = null!;

    public string OrganizationId { get; set; } = null!;

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

    public DateTime? Deleted { get; set; }

    public string? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}
