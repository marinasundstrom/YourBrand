
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class ProjectTeam : AuditableEntity, IHasTenant, ISoftDelete
{
    private ProjectTeam()
    {
    }

    public ProjectTeam(Team team)
    {
        Team = team;
    }

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public TenantId TenantId { get; set; }

    public Project Project { get; set; } = null!;

    public Team Team { get; set; } = null!;

    public string ProjectId { get; set; } = null!;

    public string TeamId { get; set; } = null!;

    public DateTime? Deleted { get; set; }

    public string? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}