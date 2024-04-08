
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class User : AuditableEntity, IHasTenant, ISoftDelete
{
    public UserId Id { get; set; } = null!;

    public TenantId TenantId { get; set; } = null!;

    public OrganizationId OrganizationId { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? DisplayName { get; set; }

    public string SSN { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public bool Hidden { get; set; }
}