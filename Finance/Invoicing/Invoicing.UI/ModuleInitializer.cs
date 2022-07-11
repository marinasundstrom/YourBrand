using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;
using YourBrand.Invoicing.Client;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Modules;
using Microsoft.Extensions.Localization;

namespace YourBrand.Invoicing;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddInvoicingClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/invoicing/");
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
        group.CreateItem("invoices", () => resources["Invoices"], MudBlazor.Icons.Material.Filled.InsertDriveFile, "/invoices");
    }
}