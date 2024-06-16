using YourBrand.Application.BrandProfiles;
using YourBrand.Application.Widgets;
using YourBrand.Domain.Entities;

namespace YourBrand.Application;

public static class Mapper
{
    public static WidgetDto ToDto(this Widget widget) => new(widget.Id, widget.WidgetId, widget.WidgetAreaId, widget.UserId, widget?.Settings?.RootElement.ToString());

    public static BrandProfileDto ToDto(this BrandProfile brandProfile) => new(brandProfile.Id, brandProfile.Name, brandProfile.Description,
        new BrandColorsDto(brandProfile.Colors.Light?.ToDto(), brandProfile.Colors.Dark?.ToDto()));

    public static BrandColorPaletteDto ToDto(this BrandColorPalette brandColorPalette) => new(brandColorPalette.BackgroundColor, brandColorPalette.AppbarBackgroundColor, brandColorPalette.PrimaryColor, brandColorPalette.SecondaryColor);
}