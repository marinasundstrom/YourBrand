using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal.Navigation;

namespace YourBrand.Sales.Subscriptions;

public static class ServiceExtensions
{
    public static IServiceProvider UseSubscriptions(this IServiceProvider services)
    {
        services.InitNavBar();

        return services;
    }

    private static void InitNavBar(this IServiceProvider services)
    {
        var navManager = services
           .GetRequiredService<NavManager>();

        var t = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("sales") ?? navManager.CreateGroup("sales", () => t["Sales"]);
        group.RequiresAuthorization = true;

        group.CreateItem("subscriptions", () => t["Subscriptions"], MudBlazor.Icons.Material.Filled.Subscriptions, "/subscriptions");
    }
}