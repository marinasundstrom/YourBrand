
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Ticketing.Domain.Entities;

public class TeamMembership : Entity<string>, IAuditable, IHasTenant, IHasOrganization, ISoftDeletable
{
    protected TeamMembership()
    {

    }

    internal TeamMembership(User user)
    {
        User = user;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public Team Team { get; set; } = null!;

    public string TeamId { get; set; } = null!;

    public User User { get; set; } = null!;

    public UserId UserId { get; set; } = null!;

    public User? CreatedBy { get; set; }
    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}