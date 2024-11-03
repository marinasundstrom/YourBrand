
using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Ticketing.Domain.Entities;

public class ProjectGroup : Entity<string>, IAuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletable
{
    public ProjectGroup()
    {
    }

    public ProjectGroup(string id) : base(id)
    {
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public Project? Project { get; set; } = null!;

    public List<Project> Projects { get; set; } = new List<Project>();

    public User? CreatedBy { get; set; }
    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}