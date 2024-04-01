using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal.AppBar;

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
        var appBarTray = services
            .GetRequiredService<IAppBarTrayService>();

        var t = services.GetRequiredService<IStringLocalizer<AppBar.AppBar>>();

        var themeSelectorId = "Shell.ThemeSelector";

        appBarTray.AddItem(new AppBarTrayItem(themeSelectorId, () => t["Theme"], typeof(ThemeSelector)));
    }
}