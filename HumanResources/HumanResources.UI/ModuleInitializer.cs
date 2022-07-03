using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;
using YourBrand.HumanResources.Client;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Modules;

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

        var group = navManager.AddGroup("human-resources", "Human Resources");
        group.AddItem("persons", "Persons", MudBlazor.Icons.Material.Filled.Person, "/users");
        group.AddItem("teams", "Teams", MudBlazor.Icons.Material.Filled.People, "/teams");
    }
}