using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

using YourBrand.Inventory.Client;
using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;

namespace YourBrand.Inventory;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.TryAddTransient<CustomAuthorizationMessageHandler>();

        services.AddInventoryClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.InventoryServiceUrl}/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static Task ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("inventory") ?? navManager.CreateGroup("inventory", () => resources["Inventory"]);
        group.RequiresAuthorization = true;

        group.CreateItem("items", () => resources["Items"], MudBlazor.Icons.Material.Filled.ListAlt, "/inventory/items");
        group.CreateItem("warehouses", () => resources["Warehouses"], MudBlazor.Icons.Material.Filled.ListAlt, "/inventory/warehouses");
        group.CreateItem("sites", () => resources["Sites"], MudBlazor.Icons.Material.Filled.ListAlt, "/inventory/sites");

        return Task.CompletedTask;
    }
}