using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddHttpClient(nameof(YourBrand.TimeReport.Client.ITimeSheetsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourBrand.TimeReport.Client.ITimeSheetsClient>((http, sp) => new YourBrand.TimeReport.Client.TimeSheetsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourBrand.TimeReport.Client.IProjectsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourBrand.TimeReport.Client.IProjectsClient>((http, sp) => new YourBrand.TimeReport.Client.ProjectsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourBrand.TimeReport.Client.IActivitiesClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourBrand.TimeReport.Client.IActivitiesClient>((http, sp) => new YourBrand.TimeReport.Client.ActivitiesClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourBrand.TimeReport.Client.IReportsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourBrand.TimeReport.Client.IReportsClient>((http, sp) => new YourBrand.TimeReport.Client.ReportsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourBrand.TimeReport.Client.IUsersClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourBrand.TimeReport.Client.IUsersClient>((http, sp) => new YourBrand.TimeReport.Client.UsersClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourBrand.TimeReport.Client.IExpensesClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourBrand.TimeReport.Client.IExpensesClient>((http, sp) => new YourBrand.TimeReport.Client.ExpensesClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        return services;
    }
}