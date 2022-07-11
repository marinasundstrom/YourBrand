using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;
using YourBrand.HumanResources.Client;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Modules;
using Microsoft.Extensions.Localization;

namespace YourBrand.HumanResources;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddHumanResourcesClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/humanresources/");
        }, builder => {
            //builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.CreateGroup("human-resources", () => resources["Human resources"]);
        group.CreateItem("organization", () => resources["Organization"], MudBlazor.Icons.Material.Filled.House, "/hr/organization");
        group.CreateItem("persons", () => resources["Persons"], MudBlazor.Icons.Material.Filled.Person, "/hr/persons");
        group.CreateItem("teams", () => resources["Teams"], MudBlazor.Icons.Material.Filled.People, "/hr/teams");
    }
}