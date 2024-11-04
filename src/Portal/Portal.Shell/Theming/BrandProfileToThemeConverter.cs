using MudBlazor;

using YourBrand.AppService.Client;

namespace YourBrand.Portal.Theming;

public static class BrandProfileToThemeConverter
{
    public static MudTheme Convert(BrandProfile brandProfile)
    {
        var theme = new MudTheme();

        if (brandProfile.Colors.Light is BrandColorPalette colorPalette)
        {
            if (colorPalette.BackgroundColor is not null)
            {
                theme.PaletteLight.Background = colorPalette.BackgroundColor;
            }
            if (colorPalette.AppbarBackgroundColor is not null)
            {
                theme.PaletteLight.AppbarBackground = colorPalette.AppbarBackgroundColor;
            }
            if (colorPalette.PrimaryColor is not null)
            {
                theme.PaletteLight.Primary = colorPalette.PrimaryColor;
            }
            if (colorPalette.SecondaryColor is not null)
            {
                theme.PaletteLight.Secondary = colorPalette.SecondaryColor;
            }
        }

        if (brandProfile.Colors.Dark is BrandColorPalette colorPalette2)
        {
            if (colorPalette2.BackgroundColor is not null)
            {
                theme.PaletteDark.Background = colorPalette2.BackgroundColor;
            }
            if (colorPalette2.AppbarBackgroundColor is not null)
            {
                theme.PaletteDark.AppbarBackground = colorPalette2.AppbarBackgroundColor;
            }
            if (colorPalette2.PrimaryColor is not null)
            {
                theme.PaletteDark.Primary = colorPalette2.PrimaryColor;
            }
            if (colorPalette2.SecondaryColor is not null)
            {
                theme.PaletteDark.Secondary = colorPalette2.SecondaryColor;
            }
        }

        return theme;
    }
}