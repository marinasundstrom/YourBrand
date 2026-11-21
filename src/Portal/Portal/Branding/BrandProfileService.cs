using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;

using YourBrand.AppService.Client;
using YourBrand.Portal.Theming;

namespace YourBrand.Portal.Branding;

public sealed class BrandProfileService(IThemeManager themeManager, IBrandProfileClient brandProfileClient, ILogger<BrandProfileService> logger)
{
    public async Task LoadBrandProfileAsync()
    {
        themeManager.SetTheme(new Theme() {  Logo = "/logo.svg" });

        try
        {
            var brandProfile = await brandProfileClient.GetBrandProfileAsync();

            if (brandProfile is not null)
            {
                themeManager.SetTheme(brandProfile.Theme);
            }
        }
        catch
        {
            logger.LogDebug("Could not load brand profile.");
        }
    }
}
