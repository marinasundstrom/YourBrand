using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public class Project : AggregateRoot<ProjectId>, IHasTenant, IHasOrganization, IAuditable
{
    readonly HashSet<Team> _teams = new HashSet<Team>();
    readonly HashSet<ProjectMembership> _memberships = new HashSet<ProjectMembership>();
    readonly HashSet<ProjectTeam> _projectTeams = new HashSet<ProjectTeam>();

    public Project()
    {
    }

    public Project(ProjectId id) : base(id)
    {
    }

    public Project(ProjectId id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public User? CreatedBy { get; set; } = null!;
    public UserId? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public HashSet<TicketType> TicketTypes { get; } = new HashSet<TicketType>();

    public HashSet<TicketStatus> TicketStatuses { get; } = new HashSet<TicketStatus>();

    public HashSet<TicketCategory> TicketCategories { get; } = new HashSet<TicketCategory>();

    public HashSet<Tag> Tags { get; } = new HashSet<Tag>();

    public IReadOnlyCollection<Team> Teams => _teams;

    public IReadOnlyCollection<ProjectMembership> Memberships => _memberships;

    public IReadOnlyCollection<ProjectTeam> ProjectTeams => _projectTeams;

    public void AddTeam(Team team) => _projectTeams.Add(new ProjectTeam(team));

    public void RemoveTeam(Team team) => _teams.Remove(team);

}