
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

public class TeamMembership : AuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletable
{
    protected TeamMembership()
    {
    }

    internal TeamMembership(User user) : base(Guid.NewGuid().ToString())
    {
        User = user;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public Team Team { get; set; } = null!;

    public string TeamId { get; set; } = null!;

    public User User { get; set; } = null!;

    public UserId UserId { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}