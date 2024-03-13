using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal;
using YourBrand.Analytics.Client;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using Microsoft.Extensions.Localization;
using YourBrand.Portal.Widgets;

namespace YourBrand.Analytics;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddAnalyticsClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.AnalyticsServiceUrl}/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var t = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("analytics") ?? navManager.CreateGroup("analytics", options =>
        {
            options.SetName(() => t["Analytics"]);
            options.RequiresAuthorization = true;
        });

        group.CreateItem("engagement", () => t["Engagement"], MudBlazor.Icons.Material.Filled.Analytics, "/analytics/engagement");

        var widgetService =
            services.GetRequiredService<IWidgetService>();

        widgetService.RegisterWidget(new Widget("analytics.engagements", "Engagements", typeof(EngagementsWidget))
        {
            Size = WidgetSize.Medium
        });
    }
}