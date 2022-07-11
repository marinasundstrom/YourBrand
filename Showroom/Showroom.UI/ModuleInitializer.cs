using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Shared;
using YourBrand.Showroom.Client;

namespace YourBrand.Showroom;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddShowroomClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/showroom/");
        }, builder => {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("sales") ?? navManager.CreateGroup("sales", () => resources["Sales"]);
        group.CreateItem("cvs", () => resources["CVs"], MudBlazor.Icons.Material.Filled.Person, "/consultants");
    }
}