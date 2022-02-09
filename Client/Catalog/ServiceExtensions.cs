using System;
using System.Globalization;

using Catalog.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.JSInterop;

using MudBlazor.Services;

namespace Catalog;

public static class ServiceExtensions
{
    public static IServiceCollection AddApp(this IServiceCollection services)
    {
        //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("sv");
        //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("sv");

        services.AddLocalization();

        services.AddMudServices();

        services.AddScoped<INotificationService, NotificationService>();

        services.AddSingleton<IFilePickerService, FilePickerService>();

        services.AddScoped<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Catalog.Client.IClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Catalog.Client.IClient>((http, sp) => new Catalog.Client.Client(http));

        services.AddHttpClient(nameof(Catalog.Client.IItemsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Catalog.Client.IItemsClient>((http, sp) => new Catalog.Client.ItemsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Catalog.Client.IDoSomethingClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Catalog.Client.IDoSomethingClient>((http, sp) => new Catalog.Client.DoSomethingClient(http));

        services.AddHttpClient(nameof(Catalog.Client.ISearchClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Catalog.Client.ISearchClient>((http, sp) => new Catalog.Client.SearchClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Catalog.Client.INotificationsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Catalog.Client.INotificationsClient>((http, sp) => new Catalog.Client.NotificationsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        SetupTimeReport(services);

        return services;
    }

    private static void SetupTimeReport(IServiceCollection services)
    {
        services.AddHttpClient(nameof(TimeReport.Client.ITimeSheetsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
                .AddTypedClient<TimeReport.Client.ITimeSheetsClient>((http, sp) => new TimeReport.Client.TimeSheetsClient(http));

        services.AddHttpClient(nameof(TimeReport.Client.IProjectsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
                .AddTypedClient<TimeReport.Client.IProjectsClient>((http, sp) => new TimeReport.Client.ProjectsClient(http));

        services.AddHttpClient(nameof(TimeReport.Client.IActivitiesClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
                .AddTypedClient<TimeReport.Client.IActivitiesClient>((http, sp) => new TimeReport.Client.ActivitiesClient(http));

        services.AddHttpClient(nameof(TimeReport.Client.IReportsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
                .AddTypedClient<TimeReport.Client.IReportsClient>((http, sp) => new TimeReport.Client.ReportsClient(http));

        services.AddHttpClient(nameof(TimeReport.Client.IUsersClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
                .AddTypedClient<TimeReport.Client.IUsersClient>((http, sp) => new TimeReport.Client.UsersClient(http));

        services.AddHttpClient(nameof(TimeReport.Client.IExpensesClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        })
                .AddTypedClient<TimeReport.Client.IExpensesClient>((http, sp) => new TimeReport.Client.ExpensesClient(http));
    }

    public static async Task UseApp(this IServiceProvider serviceProvider)
    {
        CultureInfo culture;
        var js = serviceProvider.GetRequiredService<IJSRuntime>();
        var result = await js.InvokeAsync<string>("blazorCulture.get");

        if (result != null)
        {
            culture = new CultureInfo(result);
        }
        else
        {
            culture = new CultureInfo("en-US");
            await js.InvokeVoidAsync("blazorCulture.set", "en-US");
        }

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}