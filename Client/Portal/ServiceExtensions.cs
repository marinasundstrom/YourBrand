using System;
using System.Globalization;

using Skynet.Portal.Services;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.JSInterop;

using MudBlazor.Services;

namespace Skynet.Portal;

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

        services.AddHttpClient(nameof(Skynet.Client.IClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Skynet.Client.IClient>((http, sp) => new Skynet.Client.Client(http));

        services.AddHttpClient(nameof(Skynet.Client.IItemsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Skynet.Client.IItemsClient>((http, sp) => new Skynet.Client.ItemsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Skynet.Client.IDoSomethingClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Skynet.Client.IDoSomethingClient>((http, sp) => new Skynet.Client.DoSomethingClient(http));

        services.AddHttpClient(nameof(Skynet.Client.ISearchClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Skynet.Client.ISearchClient>((http, sp) => new Skynet.Client.SearchClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Skynet.Client.INotificationsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Skynet.Client.INotificationsClient>((http, sp) => new Skynet.Client.NotificationsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        SetupTimeReport(services);

        return services;
    }

    private static void SetupTimeReport(IServiceCollection services)
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