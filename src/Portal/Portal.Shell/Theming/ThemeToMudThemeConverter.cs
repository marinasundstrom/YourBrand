using MudBlazor;

using YourBrand.AppService.Client;

namespace YourBrand.Portal.Theming;

public static class ThemeToMudThemeConverter
{
    public static MudTheme Convert(Theme theme)
    {
        var mudTheme = new MudTheme();

        if (theme?.ColorSchemes?.Light is ThemeColorScheme lightColorScheme)
        {
            if (lightColorScheme.BackgroundColor is not null)
            {
                mudTheme.PaletteLight.Background = lightColorScheme.BackgroundColor;
            }
            if (lightColorScheme.AppbarBackgroundColor is not null)
            {
                mudTheme.PaletteLight.AppbarBackground = lightColorScheme.AppbarBackgroundColor;
            }
            if (lightColorScheme.AppbarTextColor is not null)
            {
                mudTheme.PaletteLight.AppbarText = lightColorScheme.AppbarTextColor;
            }
            if (lightColorScheme.PrimaryColor is not null)
            {
                mudTheme.PaletteLight.Primary = lightColorScheme.PrimaryColor;
            }
            if (lightColorScheme.SecondaryColor is not null)
            {
                mudTheme.PaletteLight.Secondary = lightColorScheme.SecondaryColor;
            }
            if (lightColorScheme.TertiaryColor is not null)
            {
                mudTheme.PaletteLight.Tertiary = lightColorScheme.TertiaryColor;
            }
            if (lightColorScheme.InfoColor is not null)
            {
                mudTheme.PaletteLight.Info = lightColorScheme.InfoColor;
            }
            if (lightColorScheme.SuccessColor is not null)
            {
                mudTheme.PaletteLight.Success = lightColorScheme.SuccessColor;
            }
            if (lightColorScheme.WarningColor is not null)
            {
                mudTheme.PaletteLight.Warning = lightColorScheme.WarningColor;
            }
            if (lightColorScheme.ErrorColor is not null)
            {
                mudTheme.PaletteLight.Error = lightColorScheme.ErrorColor;
            }
            if (lightColorScheme.TextPrimary is not null)
            {
                mudTheme.PaletteLight.TextPrimary = lightColorScheme.TextPrimary;
            }
            if (lightColorScheme.TextSecondary is not null)
            {
                mudTheme.PaletteLight.TextSecondary = lightColorScheme.TextSecondary;
            }
            if (lightColorScheme.TextDisabled is not null)
            {
                mudTheme.PaletteLight.TextDisabled = lightColorScheme.TextDisabled;
            }
        }

        if (theme?.ColorSchemes?.Dark is ThemeColorScheme darkColorScheme)
        {
            if (darkColorScheme.BackgroundColor is not null)
            {
                mudTheme.PaletteDark.Background = darkColorScheme.BackgroundColor;
            }
            if (darkColorScheme.AppbarBackgroundColor is not null)
            {
                mudTheme.PaletteDark.AppbarBackground = darkColorScheme.AppbarBackgroundColor;
            }
            if (darkColorScheme.AppbarTextColor is not null)
            {
                mudTheme.PaletteDark.AppbarText = darkColorScheme.AppbarTextColor;
            }
            if (darkColorScheme.PrimaryColor is not null)
            {
                mudTheme.PaletteDark.Primary = darkColorScheme.PrimaryColor;
            }
            if (darkColorScheme.SecondaryColor is not null)
            {
                mudTheme.PaletteDark.Secondary = darkColorScheme.SecondaryColor;
            }
            if (darkColorScheme.TertiaryColor is not null)
            {
                mudTheme.PaletteDark.Tertiary = darkColorScheme.TertiaryColor;
            }
            if (darkColorScheme.ActionDefaultColor is not null)
            {
                mudTheme.PaletteDark.ActionDefault = darkColorScheme.ActionDefaultColor;
            }
            if (darkColorScheme.ActionDisabledColor is not null)
            {
                mudTheme.PaletteDark.ActionDisabled = darkColorScheme.ActionDisabledColor;
            }
            if (darkColorScheme.InfoColor is not null)
            {
                mudTheme.PaletteDark.Info = darkColorScheme.InfoColor;
            }
            if (darkColorScheme.SuccessColor is not null)
            {
                mudTheme.PaletteDark.Success = darkColorScheme.SuccessColor;
            }
            if (darkColorScheme.WarningColor is not null)
            {
                mudTheme.PaletteDark.Warning = darkColorScheme.WarningColor;
            }
            if (darkColorScheme.ErrorColor is not null)
            {
                mudTheme.PaletteDark.Error = darkColorScheme.ErrorColor;
            }
            if (darkColorScheme.TextPrimary is not null)
            {
                mudTheme.PaletteDark.TextPrimary = darkColorScheme.TextPrimary;
            }
            if (darkColorScheme.TextSecondary is not null)
            {
                mudTheme.PaletteDark.TextSecondary = darkColorScheme.TextSecondary;
            }
            if (darkColorScheme.TextDisabled is not null)
            {
                mudTheme.PaletteDark.TextDisabled = darkColorScheme.TextDisabled;
            }
        }

        return mudTheme;
    }
}