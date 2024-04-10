using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal.AppBar;

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

        var notificationsId = "Shell.Notifications";

        var notifications = new AppBarTrayItem(notificationsId, () => t["Notifications"], typeof(Notifications));
        notifications.RequiresAuthorization = true;

        appBarTray.AddItem(notifications);
    }
}