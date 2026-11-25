
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

/// <summary>
/// Represents membership-specific Task settings for a particular member
/// </summary>
public class ProjectMembershipTask : AuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletableWithAudit<User>
{
    public ProjectMembershipTask() : base(Guid.NewGuid().ToString())
    {
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public Project Project { get; set; } = null!;

    public Project ProjectMembership { get; set; } = null!;

    public Task Task { get; set; } = null!;

    public List<TaskCharge> Charges { get; set; } = [];

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}