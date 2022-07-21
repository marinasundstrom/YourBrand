using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.Portal;

namespace YourBrand.TimeReport;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddScoped<CustomAuthorizationMessageHandler>();

        services.AddTimeReportClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.TimeReportServiceUrl}/");
        }, builder => {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.CreateGroup("project-management", () => resources["Project management"]);
        group.RequiresAuthorization = true;

        group.CreateItem("projects", () => resources["Projects"], MudBlazor.Icons.Material.Filled.List, "/projects");
        group.CreateItem("report-time", () => resources["Report time"], MudBlazor.Icons.Material.Filled.AccessTime, "/timesheet");
        group.CreateItem("reports", () => resources["Reports"], MudBlazor.Icons.Material.Filled.ListAlt, "/reports");
    }
}