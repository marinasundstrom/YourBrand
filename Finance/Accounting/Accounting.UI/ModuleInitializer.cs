using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;
using YourBrand.Accounting.Client;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;

namespace YourBrand.Accounting;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddAccountingClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/accounting/");
        }, builder => {
            //builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var group = navManager.GetGroup("finance") ?? navManager.CreateGroup("finance", "Finance");
        group.CreateItem("accounting", "Accounting", MudBlazor.Icons.Material.Filled.List, "/verifications");
    }
}