using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;
using YourBrand.Invoices.Client;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Modules;

namespace YourBrand.Invoices;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddInvoicesClients((sp, httpClient) => {
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

        var group = navManager.GetGroup("finance") ?? navManager.CreateGroup("finance", "Finance");
        group.CreateItem("invoices", "Invoices", MudBlazor.Icons.Material.Filled.InsertDriveFile, "/invoices");
    }
}