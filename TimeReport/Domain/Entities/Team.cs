
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class Team : AuditableEntity, ISoftDelete
{
    readonly List<User> _members = new List<User>();
    readonly List<TeamMembership> _memberships = new List<TeamMembership>();
    readonly List<Project> _projects = new List<Project>();
    readonly List<ProjectTeam> _projectTeams = new List<ProjectTeam>();
    readonly List<TeamMembership> _teamMemberships = new List<TeamMembership>();

    protected Team()
    {

    }

    public Team(string name, string? description)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Description = description;
    }

    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Organization Organization { get; set; } = null!;

    public IReadOnlyCollection<User> Members => _members.AsReadOnly();

    public IReadOnlyCollection<TeamMembership> Memberships => _memberships.AsReadOnly();

    public IReadOnlyCollection<Project> Projects => _projects.AsReadOnly();

    public IReadOnlyCollection<ProjectTeam> ProjectTeams => _projectTeams.AsReadOnly();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
