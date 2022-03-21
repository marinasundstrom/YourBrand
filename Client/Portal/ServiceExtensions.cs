using System;
using System.Globalization;

using Skynet.Portal.Services;
using Skynet.Portal.Shared;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.JSInterop;

using MudBlazor.Services;

namespace Skynet.Portal;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("sv");
        //CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("sv");

        services.AddLocalization();

        services.AddMudServices();

        services.AddScoped<INotificationService, NotificationService>();

        services.AddSingleton<IFilePickerService, FilePickerService>();

        services.AddScoped<CustomAuthorizationMessageHandler>();

        services.AddClients();

        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services) 
    {
        services.AddHttpClient(nameof(Skynet.AppService.Client.IClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Skynet.AppService.Client.IClient>((http, sp) => new Skynet.AppService.Client.Client(http));

        services.AddHttpClient(nameof(Skynet.AppService.Client.IItemsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Skynet.AppService.Client.IItemsClient>((http, sp) => new Skynet.AppService.Client.ItemsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Skynet.AppService.Client.IDoSomethingClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Skynet.AppService.Client.IDoSomethingClient>((http, sp) => new Skynet.AppService.Client.DoSomethingClient(http));

        services.AddHttpClient(nameof(Skynet.AppService.Client.ISearchClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Skynet.AppService.Client.ISearchClient>((http, sp) => new Skynet.AppService.Client.SearchClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Skynet.AppService.Client.INotificationsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        })
        .AddTypedClient<Skynet.AppService.Client.INotificationsClient>((http, sp) => new Skynet.AppService.Client.NotificationsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(IdentityService.Client.IUsersClient) + "2", (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"https://identity.local/");
        })
        .AddTypedClient<IdentityService.Client.IUsersClient>((http, sp) => new IdentityService.Client.UsersClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(IdentityService.Client.IRolesClient) + "2", (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"https://identity.local/");
        })
       .AddTypedClient<IdentityService.Client.IRolesClient>((http, sp) => new IdentityService.Client.RolesClient(http))
       .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        return services;
    }

    public static async Task Localize(this IServiceProvider serviceProvider)
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