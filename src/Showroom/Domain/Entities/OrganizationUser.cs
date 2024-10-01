using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class OrganizationUser : AuditableEntity, IHasTenant, ISoftDeletable
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();

    public TenantId TenantId { get; set; }

    public Organization Organization { get; set; } = null!;

    public OrganizationId OrganizationId { get; set; } = null!;

    public User User { get; set; } = null!;

    public UserId UserId { get; set; } = null!;

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

    public User? DeletedBy { get; set; }
}