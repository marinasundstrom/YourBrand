using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Navigation;
using YourBrand.Portal.Shared;

namespace YourBrand.TimeReport;

public static class ServiceExtensions
{
    public static IServiceCollection AddTimeReport(this IServiceCollection services)
    {
        services.AddClients();
        
        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddTimeReportClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/timereport/");
        }, builder => {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });

        return services;
    }

    public static IServiceProvider UseTimeReport(this IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var group = navManager.AddGroup("project-management", "Project Management");
        group.AddItem("projects", "Projects", MudBlazor.Icons.Material.Filled.List, "/projects");
        group.AddItem("report-time", "Report time", MudBlazor.Icons.Material.Filled.AccessTime, "/timesheet");
        group.AddItem("reports", "Reports", MudBlazor.Icons.Material.Filled.ListAlt, "/reports");

        return services;
    }
}