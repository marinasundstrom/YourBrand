using YourBrand.Domain.Common;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Domain.Entities;

public class BrandProfile : AuditableEntity<string>, ISoftDeletableWithAudit<User>, IHasTenant
{
    protected BrandProfile()
    {

    }

    public BrandProfile(string name, string? description = null) : base(Guid.NewGuid().ToString())
    {
        Name = name;
        Description = description;
    }

    public TenantId TenantId { get; set; } = null!;

    public OrganizationId? OrganizationId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public BrandColors Colors { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}

public class BrandColors
{
    public BrandColorPalette Light { get; set; }

    public BrandColorPalette Dark { get; set; }
}

public class BrandColorPalette
{
    public string? BackgroundColor { get; set; } = null!;
    public string? AppbarBackgroundColor { get; set; } = null!;
    public string? PrimaryColor { get; set; } = null!;
    public string? SecondaryColor { get; set; } = null!;
}