using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.RotRutService.Client;

namespace YourBrand.RotRutService;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddRotRutClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.RotRutServiceUrl}/");
        }, builder =>
        {
            //builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("finance") ?? navManager.CreateGroup("finance", () => resources["Finance"]);
        group.RequiresAuthorization = true;

        var group2 = group.GetGroup("accounting") ?? group.CreateGroup("accounting", () => resources["Accounting"]);

        group2.CreateItem("rotrut", () => resources["Rot & Rut ärenden"], MudBlazor.Icons.Material.Filled.InsertDriveFile, "/rotrut");
    }
}