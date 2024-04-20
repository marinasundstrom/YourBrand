using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal.AppBar;
using YourBrand.Portal.Navigation;

namespace YourBrand.Portal.Theming;

public static class ServiceProviderExtensions
{
    public static IServiceProvider UseTheming(this IServiceProvider services)
    {
        AddAppBarTrayItems(services);

        return services;
    }

    private static void AddAppBarTrayItems(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("administration") ?? navManager.CreateGroup("administration", () => resources["Administration"]) ?? navManager.CreateGroup("administration", () => resources["Administration"]);
        group.RequiresAuthorization = true;

        group.CreateItem("brand-profile", () => resources["Brand Profile"], MudBlazor.Icons.Material.Filled.BrandingWatermark, "/brandprofile");

        var appBarTray = services
            .GetRequiredService<IAppBarTrayService>();

        var t = services.GetRequiredService<IStringLocalizer<ColorSchemeSelector>>();

        var ColorSchemeSelectorId = "Shell.ColorSchemeSelector";

        appBarTray.AddItem(new AppBarTrayItem(ColorSchemeSelectorId, () => t["ColorScheme"], typeof(ColorSchemeSelector)));
    }
}