using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using MudBlazor;

using YourBrand.Portal.AppBar;
using YourBrand.Portal.Theming;

namespace YourBrand.Portal.Notifications;

public static class ServiceProviderExtensions
{
    public static IServiceProvider UseNotifications(this IServiceProvider services)
    {
        AddAppBarTrayItems(services);

        return services;
    }

    private static void AddAppBarTrayItems(IServiceProvider services)
    {
        var appBarTray = services
            .GetRequiredService<IAppBarTrayService>();

        var t = services.GetRequiredService<IStringLocalizer<Notifications>>();

        var themeSelectorId = "Shell.Notifications";

        appBarTray.AddItem(new AppBarTrayItem(themeSelectorId, () => t["Notifications"], typeof(Notifications)));
    }
}