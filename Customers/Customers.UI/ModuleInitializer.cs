using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;
using YourBrand.Customers.Client;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using Microsoft.Extensions.Localization;

namespace YourBrand.Customers;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddCustomersClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/customers/");
        }, builder => {
            //builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("customer-relations") ?? navManager.CreateGroup("customer-relations", () => resources["Customer relations"]);
        group.CreateItem("persons", () => resources["Persons"], MudBlazor.Icons.Material.Filled.Person, "/customers/persons");
    }
}