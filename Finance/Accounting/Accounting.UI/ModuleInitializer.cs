using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal;
using YourBrand.Accounting.Client;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using Microsoft.Extensions.Localization;

namespace YourBrand.Accounting;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddScoped<CustomAuthorizationMessageHandler>();

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

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("accounting") ?? navManager.CreateGroup("accounting", () => resources["Accounting"]);
        group.RequiresAuthorization = true;

        group.CreateItem("accounts", () => resources["Accounts"], MudBlazor.Icons.Material.Filled.List, "/accounts");
        group.CreateItem("ledger", () => resources["Ledger"], MudBlazor.Icons.Material.Filled.List, "/ledger");
        group.CreateItem("verifications", () => resources["Verifications"], MudBlazor.Icons.Material.Filled.List, "/verifications");
    }
}