using System;

using YourBrand.Messenger.Client;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using YourBrand.Portal.Shared;

namespace YourBrand.Messenger;

public static class ServiceExtensions
{
    public static IServiceCollection AddMessenger(this IServiceCollection services)
    {
        services.AddMessengerClients((sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/messenger/");
        }, builder => builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>());

        return services;
    }
}

