using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class OrganizationUser : AuditableEntity, IHasTenant, ISoftDelete
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public TenantId TenantId { get; set; }

    public Organization Organization { get; set; } = null!;

    public string OrganizationId { get; set; } = null!;

    public User User { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public DateTime? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}