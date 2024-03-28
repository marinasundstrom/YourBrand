using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Ticketing;
using YourBrand.Ticketing.Client;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Modules;
using Microsoft.Extensions.Localization;
using YourBrand.Portal;
using YourBrand.Portal.Widgets;

namespace YourBrand.Ticketing;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddTicketingClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.CustomerServiceServiceUrl}/");
        }, builder =>
        {
            //builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var t = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("customer-relations") ?? navManager.CreateGroup("customer-relations", () => t["Customer relations"]);
        group.RequiresAuthorization = true;

        var group2 = group.GetGroup("customer-support") ?? group.CreateGroup("customer-support", () => t["Support"],  MudBlazor.Icons.Material.Filled.Support);

        group2.CreateItem("board", () => t["Board"], MudBlazor.Icons.Material.Filled.TableView, "/Tickets/Board");

        group2.CreateItem("tickets", () => t["Tickets"], MudBlazor.Icons.Material.Filled.InsertDriveFile, "/Tickets");

        var widgetService =
            services.GetRequiredService<IWidgetService>();

        widgetService.RegisterWidget(new Widget("tickets.overview", "Tickets", typeof(TicketsWidget))
        {
            Size = WidgetSize.Medium
        });
    }
}