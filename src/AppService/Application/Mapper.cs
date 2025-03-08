using YourBrand.Application.BrandProfiles;
using YourBrand.Application.Themes;
using YourBrand.Application.Widgets;
using YourBrand.Domain.Entities;

namespace YourBrand.Application;

public static class Mapper
{
    public static WidgetDto ToDto(this Widget widget) => new(widget.Id, widget.WidgetId, widget.WidgetAreaId, widget.UserId, widget?.Settings?.RootElement.ToString());

    public static ThemeDto ToDto(this Theme theme) => new(theme.Id, theme.Name, theme.Description, theme.Dense.GetValueOrDefault(),
        new ThemeColorsDto(theme.Colors.Light?.ToDto(), theme.Colors.Dark?.ToDto()));

    public static ThemeColorPaletteDto ToDto(this ThemeColorPalette colorPalette) => new(colorPalette.BackgroundColor, colorPalette.AppbarBackgroundColor, colorPalette.PrimaryColor, colorPalette.SecondaryColor);

    public static BrandProfileDto ToDto(this BrandProfile brandProfile) => new(brandProfile.Id, brandProfile.Name, brandProfile.Description,
        brandProfile.Theme?.ToDto());
}