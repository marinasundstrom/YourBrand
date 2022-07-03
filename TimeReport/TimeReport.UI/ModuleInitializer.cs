using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Shared;

namespace YourBrand.TimeReport;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTimeReportClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/timereport/");
        }, builder => {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var group = navManager.AddGroup("project-management", "Project Management");
        group.AddItem("projects", "Projects", MudBlazor.Icons.Material.Filled.List, "/projects");
        group.AddItem("report-time", "Report time", MudBlazor.Icons.Material.Filled.AccessTime, "/timesheet");
        group.AddItem("reports", "Reports", MudBlazor.Icons.Material.Filled.ListAlt, "/reports");
    }
}