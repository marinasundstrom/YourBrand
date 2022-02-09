using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using Skynet.Portal.Shared;

namespace Skynet.TimeReport;

public static class ServiceExtensions
{
    public static IServiceCollection AddTimeReport(this IServiceCollection services)
    {
        services.AddClients();
        
        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(Skynet.TimeReport.Client.ITimeSheetsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<Skynet.TimeReport.Client.ITimeSheetsClient>((http, sp) => new Skynet.TimeReport.Client.TimeSheetsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Skynet.TimeReport.Client.IProjectsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<Skynet.TimeReport.Client.IProjectsClient>((http, sp) => new Skynet.TimeReport.Client.ProjectsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Skynet.TimeReport.Client.IActivitiesClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<Skynet.TimeReport.Client.IActivitiesClient>((http, sp) => new Skynet.TimeReport.Client.ActivitiesClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Skynet.TimeReport.Client.IReportsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<Skynet.TimeReport.Client.IReportsClient>((http, sp) => new Skynet.TimeReport.Client.ReportsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Skynet.TimeReport.Client.IUsersClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<Skynet.TimeReport.Client.IUsersClient>((http, sp) => new Skynet.TimeReport.Client.UsersClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Skynet.TimeReport.Client.IExpensesClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
        .AddTypedClient<Skynet.TimeReport.Client.IExpensesClient>((http, sp) => new Skynet.TimeReport.Client.ExpensesClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        return services;
    }
}