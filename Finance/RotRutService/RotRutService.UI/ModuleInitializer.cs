using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Shared;
using YourBrand.RotRutService.Client;

namespace YourBrand.RotRutService;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddRotRutClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/rotrut/");
        }, builder => {
            //builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var group = navManager.GetGroup("finance") ?? navManager.CreateGroup("finance", "Finance");
        group.CreateItem("rotrut", "Rot & Rut ärenden", MudBlazor.Icons.Material.Filled.InsertDriveFile, "/rotrut");
    }
}