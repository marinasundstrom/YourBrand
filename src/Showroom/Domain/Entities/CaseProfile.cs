using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class CaseProfile : AuditableEntity, IHasTenant, ISoftDelete
{
    public string Id { get; set; } = null!;

    public TenantId TenantId { get; set; } = null!;

    public OrganizationId OrganizationId { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public string? Presentation { get; set; }

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}