using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;
using YourBrand.Orders.Client;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using Microsoft.Extensions.Localization;

namespace YourBrand.Orders;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddOrdersClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/orders/");
        }, builder => {
            //builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("sales") ?? navManager.CreateGroup("sales", () => resources["Sales"]);
        group.CreateItem("orders", () => resources["Orders"], MudBlazor.Icons.Material.Filled.InsertDriveFile, "/orders");
    }
}