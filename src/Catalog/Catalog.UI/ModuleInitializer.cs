using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using Microsoft.Extensions.Localization;
using YourBrand.Portal.Services;
using YourBrand.Portal.AppBar;
using MudBlazor;

namespace YourBrand.Catalog;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddCatalogClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.CatalogServiceUrl}/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });

        services.AddScoped<IStoreProvider, StoreProvider>();
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
                   .GetRequiredService<NavManager>();

        var t = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("sales") ?? navManager.CreateGroup("sales", () => t["Sales"]);
        group.RequiresAuthorization = true;

        var catalogItem = group.CreateGroup("catalog", options =>
        {
            options.NameFunc = () => t["Catalog"];
            options.Icon = MudBlazor.Icons.Material.Filled.Book;
        });

        catalogItem.CreateItem("products", () => t["Products"], MudBlazor.Icons.Material.Filled.FormatListBulleted, "/products");

        catalogItem.CreateItem("categories", () => t["Categories"], MudBlazor.Icons.Material.Filled.Collections, "/products/categories");

        catalogItem.CreateItem("attributes", () => t["Attributes"], MudBlazor.Icons.Material.Filled.List, "/products/attributes");

        catalogItem.CreateItem("brands", () => t["Brands"], MudBlazor.Icons.Material.Filled.List, "/brands");

        catalogItem.CreateItem("stores", () => t["Stores"], MudBlazor.Icons.Material.Filled.Store, "/stores");

        InitAppBarTray(services);
    }

    private static void InitAppBarTray(IServiceProvider services)
    {
        var appBarTray = services
            .GetRequiredService<IAppBarTrayService>();

        var snackbar = services
            .GetRequiredService<ISnackbar>();

        var t = services.GetRequiredService<IStringLocalizer<Resources>>();

        appBarTray.AddItem(new AppBarTrayItem("store-selector", () => t["Store"], typeof(Stores.StoreSelector)));
    }
}