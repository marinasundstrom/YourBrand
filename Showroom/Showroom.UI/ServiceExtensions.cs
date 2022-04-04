using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;
using YourBrand.Showroom.Client;

namespace YourBrand.Showroom;

public static class ServiceExtensions
{
    public static IServiceCollection AddShowroom(this IServiceCollection services)
    {
        services.AddClients();
        
        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddShowroomClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        }, builder => {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });

        return services;
    }
}