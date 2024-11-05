using Microsoft.Extensions.Logging;

using YourBrand.AppService.Client;
using YourBrand.Portal.Theming;

namespace YourBrand.Portal.Branding;

public sealed class BrandProfileService(IThemeManager themeManager, IBrandProfileClient brandProfileClient, ILogger<BrandProfileService> logger)
{
    public async Task LoadBrandProfileAsync()
    {
        themeManager.SetTheme(Themes.AppTheme);

        try
        {
            var brandProfile = await brandProfileClient.GetBrandProfileAsync();

            if (brandProfile is not null)
            {
                var theme = BrandProfileToThemeConverter.Convert(brandProfile);
                themeManager.SetTheme(theme);
            }
        }
        catch (Exception)
        {
            logger.LogDebug("Could not load brand profile.");
        }
    }
}
