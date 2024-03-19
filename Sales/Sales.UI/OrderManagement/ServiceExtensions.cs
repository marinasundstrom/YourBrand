using Blazored.LocalStorage;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using MudBlazor;

using YourBrand.Portal.AppBar;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Services;
using YourBrand.Portal.Widgets;

using Store = YourBrand.Portal.Services.Store;

namespace YourBrand.Sales.OrderManagement;

public static class ServiceExtensions
{
    public static IServiceProvider UseOrderManagement(this IServiceProvider services)
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

        group.CreateItem("orders", () => t["Orders"], MudBlazor.Icons.Material.Filled.InsertDriveFile, "/orders");


        var widgetService =
            services.GetRequiredService<IWidgetService>();

        widgetService.RegisterWidget(new Widget("orders.pendingOrders", "Pending orders", typeof(PendingOrdersWidget))
        {
            Size = WidgetSize.Medium
        });
    }
}