using YourBrand.Domain.Common;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Domain.Entities;

public class Theme : AuditableEntity<string>, ISoftDeletableWithAudit<User>, IHasTenant
{
    protected Theme()
    {

    }

    public Theme(string name, string? description = null) : base(Guid.NewGuid().ToString())
    {
        Name = name;
        Description = description;
    }

    public TenantId TenantId { get; set; } = null!;

    public OrganizationId? OrganizationId { get; set; } = null!;

    public UserId? UserId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public bool? Dense { get; set; }

    public ThemeColors Colors { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}

public class ThemeColors
{
    public ThemeColorPalette Light { get; set; }

    public ThemeColorPalette Dark { get; set; }
}

public class ThemeColorPalette
{
    public string? BackgroundColor { get; set; } = null!;
    public string? AppbarBackgroundColor { get; set; } = null!;
    public string? PrimaryColor { get; set; } = null!;
    public string? SecondaryColor { get; set; } = null!;
}