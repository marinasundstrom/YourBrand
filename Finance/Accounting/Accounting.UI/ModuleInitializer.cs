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
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddAccountingClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.AccountingServiceUrl}/");
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

        var group2 = group.GetGroup("accounting") ?? group.CreateGroup("accounting", () => resources["Accounting"], MudBlazor.Icons.Material.Filled.List);

        group2.CreateItem("accounts", () => resources["Accounts"], MudBlazor.Icons.Material.Filled.List, "/accounts");
        group2.CreateItem("ledger", () => resources["Ledger"], MudBlazor.Icons.Material.Filled.List, "/ledger");
        group2.CreateItem("verifications", () => resources["Verifications"], MudBlazor.Icons.Material.Filled.List, "/verifications");
    }
}