using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;

namespace YourBrand.TimeReport;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddTimeReportClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.TimeReportServiceUrl}/");
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

        var group = navManager.CreateGroup("project-management", () => resources["Project management"]);
        group.RequiresAuthorization = true;

        group.CreateItem("projects", () => resources["Projects"], MudBlazor.Icons.Material.Filled.List, "/projects");
        group.CreateItem("report-time", () => resources["Report time"], MudBlazor.Icons.Material.Filled.AccessTime, "/timesheet");
        group.CreateItem("timesheets",options => {
            options.SetName(() => resources["Timesheets"]);
            options.Icon = MudBlazor.Icons.Material.Filled.ListAlt;
            options.Href = "/timesheets";
            options.RequiresAuthorization = true;
            options.Roles = [ "Administrator" ];
        });
        group.CreateItem("generate-reports", () => resources["GenerateReport"], MudBlazor.Icons.Material.Filled.ListAlt, "/reports");
    }
}