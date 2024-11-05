using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

using YourBrand.Marketing.Client;
using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;

namespace YourBrand.Marketing;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.TryAddTransient<CustomAuthorizationMessageHandler>();

        services.AddMarketingClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.MarketingServiceUrl}/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static Task ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("marketing") ?? navManager.CreateGroup("marketing", options =>
        {
            options.SetName(() => resources["Marketing"]);
            options.RequiresAuthorization = true;
        });

        group.CreateItem("contacts", () => resources["Contacts"], MudBlazor.Icons.Material.Filled.Person, "/marketing/contacts");

        group.CreateItem("campaigns", () => resources["Campaigns"], MudBlazor.Icons.Material.Filled.ListAlt, "/marketing/campaigns");

        group.CreateItem("discounts", () => resources["Discounts"], MudBlazor.Icons.Material.Filled.Discount, "/marketing/discounts");

        return Task.CompletedTask;
    }
}