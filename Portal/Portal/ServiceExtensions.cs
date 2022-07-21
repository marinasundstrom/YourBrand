using System;
using System.Globalization;

using YourBrand.Portal.Services;
using YourBrand.Portal;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.JSInterop;

using MudBlazor.Services;
using YourBrand.IdentityService.Client;
using YourBrand.AppService.Client;

namespace YourBrand.Portal;

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

        // Shared
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<Services.IAccessTokenProvider, AccessTokenProvider>();

        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services) 
    {
        services.AddAppServiceClients((sp, http) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/");
        }, (builder) => {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });

        services.AddIdentityServiceClients((sp, http) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"https://identity.local/");
        }, (builder) => {
               builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });

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