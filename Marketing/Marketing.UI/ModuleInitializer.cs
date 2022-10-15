using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal;
using YourBrand.Marketing.Client;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using Microsoft.Extensions.Localization;

namespace YourBrand.Marketing;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddScoped<CustomAuthorizationMessageHandler>();

        services.AddMarketingClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.MarketingServiceUrl}/");
        }, builder => {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("marketing") ?? navManager.CreateGroup("marketing", options => {
            options.SetName(() => resources["Marketing"]);
            options.RequiresAuthorization = true;
        });

        group.CreateItem("contacts", () => resources["Contacts"], MudBlazor.Icons.Material.Filled.Person, "/contacts");

        group.CreateItem("campaigns", () => resources["Campaigns"], MudBlazor.Icons.Material.Filled.ListAlt, "/campaigns");
    }
}