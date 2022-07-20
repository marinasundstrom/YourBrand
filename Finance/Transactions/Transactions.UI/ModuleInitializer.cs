using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Shared;
using YourBrand.Transactions.Client;

namespace YourBrand.Transactions;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransactionsClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/transactions/");
        }, builder => {
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

        group.CreateItem("transactions", () => resources["Transactions"], MudBlazor.Icons.Material.Filled.InsertDriveFile, "/transactions");
    }
}
