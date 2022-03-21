using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourCompany.Portal.Shared;

namespace YourCompany.TimeReport;

public static class ServiceExtensions
{
    public static IServiceCollection AddTimeReport(this IServiceCollection services)
    {
        services.AddClients();
        
        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(YourCompany.TimeReport.Client.ITimeSheetsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourCompany.TimeReport.Client.ITimeSheetsClient>((http, sp) => new YourCompany.TimeReport.Client.TimeSheetsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourCompany.TimeReport.Client.IProjectsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourCompany.TimeReport.Client.IProjectsClient>((http, sp) => new YourCompany.TimeReport.Client.ProjectsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourCompany.TimeReport.Client.IActivitiesClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourCompany.TimeReport.Client.IActivitiesClient>((http, sp) => new YourCompany.TimeReport.Client.ActivitiesClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourCompany.TimeReport.Client.IReportsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourCompany.TimeReport.Client.IReportsClient>((http, sp) => new YourCompany.TimeReport.Client.ReportsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourCompany.TimeReport.Client.IUsersClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourCompany.TimeReport.Client.IUsersClient>((http, sp) => new YourCompany.TimeReport.Client.UsersClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourCompany.TimeReport.Client.IExpensesClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<YourCompany.TimeReport.Client.IExpensesClient>((http, sp) => new YourCompany.TimeReport.Client.ExpensesClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        return services;
    }
}