using YourBrand.Application.BrandProfiles;
using YourBrand.Application.Themes;
using YourBrand.Application.Widgets;
using YourBrand.Domain.Entities;

namespace YourBrand.Application;

public static class Mapper
{
    public static WidgetDto ToDto(this Widget widget) => new(widget.Id, widget.WidgetId, widget.WidgetAreaId, widget.UserId, widget?.Settings?.RootElement.ToString());

    public static ThemeDto ToDto(this Theme theme) => new(theme.Id, theme.Name, theme.Description, theme.Title, theme.Logo, theme.Dense.GetValueOrDefault(),
        new ThemeColorSchemesDto(theme.ColorSchemes.Light?.ToDto(), theme.ColorSchemes.Dark?.ToDto()));

    public static ThemeColorSchemeDto ToDto(this ThemeColorScheme colorScheme) => new(colorScheme.Logo, colorScheme.BackgroundColor, colorScheme.AppbarBackgroundColor, colorScheme.AppbarTextColor, colorScheme.PrimaryColor, colorScheme.SecondaryColor, colorScheme.TertiaryColor, colorScheme.ActionDefaultColor, colorScheme.ActionDisabledColor, colorScheme.InfoColor, colorScheme.SuccessColor, colorScheme.WarningColor, colorScheme.ErrorColor, colorScheme.TextPrimary, colorScheme.TextSecondary, colorScheme.TextDisabled);

    public static BrandProfileDto ToDto(this BrandProfile brandProfile) => new(brandProfile.Id, brandProfile.Name, brandProfile.Description,
        brandProfile.Theme?.ToDto());
}