using MudBlazor;

using YourBrand.AppService.Client;

namespace YourBrand.Portal.Theming;

public static class ThemeToMudThemeConverter
{
    public static MudTheme Convert(Theme theme)
    {
        var mudTheme = new MudTheme();

        if (theme?.Colors?.Light is ThemeColorPalette colorPalette)
        {
            if (colorPalette.BackgroundColor is not null)
            {
                mudTheme.PaletteLight.Background = colorPalette.BackgroundColor;
            }
            if (colorPalette.AppbarBackgroundColor is not null)
            {
                mudTheme.PaletteLight.AppbarBackground = colorPalette.AppbarBackgroundColor;
            }
            if (colorPalette.PrimaryColor is not null)
            {
                mudTheme.PaletteLight.Primary = colorPalette.PrimaryColor;
            }
            if (colorPalette.SecondaryColor is not null)
            {
                mudTheme.PaletteLight.Secondary = colorPalette.SecondaryColor;
            }
        }

        if (theme?.Colors?.Dark is ThemeColorPalette colorPalette2)
        {
            if (colorPalette2.BackgroundColor is not null)
            {
                mudTheme.PaletteDark.Background = colorPalette2.BackgroundColor;
            }
            if (colorPalette2.AppbarBackgroundColor is not null)
            {
                mudTheme.PaletteDark.AppbarBackground = colorPalette2.AppbarBackgroundColor;
            }
            if (colorPalette2.PrimaryColor is not null)
            {
                mudTheme.PaletteDark.Primary = colorPalette2.PrimaryColor;
            }
            if (colorPalette2.SecondaryColor is not null)
            {
                mudTheme.PaletteDark.Secondary = colorPalette2.SecondaryColor;
            }
        }

        return mudTheme;
    }
}