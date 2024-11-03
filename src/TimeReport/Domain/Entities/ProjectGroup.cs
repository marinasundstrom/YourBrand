
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;


namespace YourBrand.TimeReport.Domain.Entities;

public class ProjectGroup : AuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletable
{
    public ProjectGroup() : base(Guid.NewGuid().ToString())
    {
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public Project? Project { get; set; } = null!;

    public List<Project> Projects { get; set; } = new List<Project>();

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}