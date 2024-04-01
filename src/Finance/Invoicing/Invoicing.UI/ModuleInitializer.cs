using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Invoicing.Client;
using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;

namespace YourBrand.Invoicing;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddInvoicingClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.InvoicingServiceUrl}/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("finance") ?? navManager.CreateGroup("finance", () => resources["Finance"]);
        group.RequiresAuthorization = true;

        group.CreateItem("invoices", () => resources["Invoices"], MudBlazor.Icons.Material.Filled.InsertDriveFile, "/invoices");
    }
}