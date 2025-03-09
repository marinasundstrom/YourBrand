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

    public string? Logo { get; set; } = null!;

    public string? Title { get; set; } = null!;

    public bool? Dense { get; set; }

    public ThemeColorSchemes ColorSchemes { get; set; }

    public string? CustomCss { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}

public class ThemeColorSchemes
{
    public ThemeColorScheme Light { get; set; }

    public ThemeColorScheme Dark { get; set; }
}

public class ThemeColorScheme
{
    public string? Logo { get; set; } = null!;
    
    public string? BackgroundColor { get; set; } = null!;
    public string? AppbarBackgroundColor { get; set; } = null!;
    public string? AppbarTextColor { get; set; } = null!;

    public string? PrimaryColor { get; set; } = null!;
    public string? SecondaryColor { get; set; } = null!;
    public string? TertiaryColor { get; set; } = null!;

    public string? ActionDefaultColor { get; set; } = null!;
    public string? ActionDisabledColor { get; set; } = null!;

    public string? InfoColor { get; set; } = null!;
    public string? SuccessColor { get; set; } = null!;
    public string? WarningColor { get; set; } = null!;
    public string? ErrorColor { get; set; } = null!;

    public string? TextPrimary { get; set; } = null!;
    public string? TextSecondary { get; set; } = null!;
    public string? TextDisabled { get; set; } = null!;
}