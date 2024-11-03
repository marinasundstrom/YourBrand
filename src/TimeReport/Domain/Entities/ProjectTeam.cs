
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

public class ProjectTeam : AuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletableWithAudit<User>
{
    protected ProjectTeam()
    {
    }

    public ProjectTeam(Team team) : base(Guid.NewGuid().ToString())
    {
        Team = team;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public Project Project { get; set; } = null!;

    public Team Team { get; set; } = null!;

    public string ProjectId { get; set; } = null!;

    public string TeamId { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}