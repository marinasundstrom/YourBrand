using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal.AppBar;
using YourBrand.Portal.Authentication;
using YourBrand.Portal.Localization;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Theming;
using YourBrand.Portal.Notifications;
using YourBrand.Portal.Widgets;

namespace YourBrand.Portal;

public static class ServiceProviderExtensions
{
    public static IServiceProvider UseShell(this IServiceProvider services)
    {
        CreateNavMenu(services);

        CreateDashboard(services);

        return services
            .UseNotifications()
            .UseTheming()
            .UseLocalization()
            .UseAuthentication();
    }

    private static void CreateNavMenu(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var t = services.GetRequiredService<IStringLocalizer<Resources>>();

        navManager.CreateItem("home", options =>
        {
            options.NameFunc = () => t["Dashboard"];
            options.Icon = MudBlazor.Icons.Material.Filled.Dashboard;
            options.Href = "/";
            options.RequiresAuthorization = false;
            options.Index = 0;
        });

        var group = navManager.GetGroup("administration") ?? navManager.CreateGroup("administration", () => t["Administration"]);

        group.CreateItem("users", options =>
        {
            options.NameFunc = () => t["Users"];
            options.Icon = MudBlazor.Icons.Material.Filled.Person;
            options.Href = "/users";
            options.RequiresAuthorization = true;
        });

        group.CreateItem("setup", options =>
        {
            options.NameFunc = () => t["SetUp"];
            options.Icon = MudBlazor.Icons.Material.Filled.Settings;
            options.Href = "/setup";
        });

        var notificationsGroup = group.CreateGroup("notifications", options =>
        {
            options.NameFunc = () => t["Notifications"];
            options.Icon = MudBlazor.Icons.Material.Filled.Notifications;
            options.Href = "/notifications";
        });

        notificationsGroup.CreateItem("send-notification", options =>
        {
            options.NameFunc = () => t["Send notification"];
            options.Icon = MudBlazor.Icons.Material.Filled.NotificationAdd;
            options.Href = "/notifications/create";
        });
    }

    private static void CreateDashboard(IServiceProvider services)
    {
        var widgetService =
            services.GetRequiredService<IWidgetService>();

        widgetService.RegisterWidget(new Widget("sample-widget", "Sample Widget", typeof(SampleWidget)));

        widgetService.RegisterWidget(new Widget("sample-widget2", "Sample Widget", typeof(SampleWidget2))
        {
            Size = WidgetSize.Small
        });

        /*
        widgetService.RegisterWidget(new Widget("sample-widget3", "Sample Widget", typeof(SampleWidget2)) {
            Size = WidgetSize.Large
        });
        */
    }
}