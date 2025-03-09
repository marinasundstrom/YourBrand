using MudBlazor;

using YourBrand.AppService.Client;

namespace YourBrand.Portal.Theming;

public static class ThemeToMudThemeConverter
{
    public static MudTheme Convert(Theme theme)
    {
        var mudTheme = new MudTheme();

        if (theme?.ColorSchemes?.Light is ThemeColorScheme colorScheme)
        {
            if (colorScheme.BackgroundColor is not null)
            {
                mudTheme.PaletteLight.Background = colorScheme.BackgroundColor;
            }
            if (colorScheme.AppbarBackgroundColor is not null)
            {
                mudTheme.PaletteLight.AppbarBackground = colorScheme.AppbarBackgroundColor;
            }
            if (colorScheme.AppbarTextColor is not null)
            {
                mudTheme.PaletteLight.AppbarText = colorScheme.AppbarTextColor;
            }
            if (colorScheme.PrimaryColor is not null)
            {
                mudTheme.PaletteLight.Primary = colorScheme.PrimaryColor;
            }
            if (colorScheme.SecondaryColor is not null)
            {
                mudTheme.PaletteLight.Secondary = colorScheme.SecondaryColor;
            }
        }

        if (theme?.ColorSchemes?.Dark is ThemeColorScheme colorScheme2)
        {
            if (colorScheme2.BackgroundColor is not null)
            {
                mudTheme.PaletteDark.Background = colorScheme2.BackgroundColor;
            }
            if (colorScheme2.AppbarBackgroundColor is not null)
            {
                mudTheme.PaletteDark.AppbarBackground = colorScheme2.AppbarBackgroundColor;
            }
            if (colorScheme2.AppbarTextColor is not null)
            {
                mudTheme.PaletteDark.AppbarText = colorScheme2.AppbarTextColor;
            }
            if (colorScheme2.PrimaryColor is not null)
            {
                mudTheme.PaletteDark.Primary = colorScheme2.PrimaryColor;
            }
            if (colorScheme2.SecondaryColor is not null)
            {
                mudTheme.PaletteDark.Secondary = colorScheme2.SecondaryColor;
            }
        }

        return mudTheme;
    }
}