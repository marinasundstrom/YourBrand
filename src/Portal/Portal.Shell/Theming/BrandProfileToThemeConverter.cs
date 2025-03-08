using MudBlazor;

using YourBrand.AppService.Client;

namespace YourBrand.Portal.Theming;

public static class BrandProfileToThemeConverter
{
    public static MudTheme Convert(BrandProfile brandProfile)
    {
        return ThemeToMudThemeConverter.Convert(brandProfile.Theme!);
    }
}
