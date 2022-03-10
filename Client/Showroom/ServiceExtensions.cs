using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using Skynet.Portal.Shared;

namespace Skynet.Showroom;

public static class ServiceExtensions
{
    public static IServiceCollection AddShowroom(this IServiceCollection services)
    {
        services.AddClients();
        
        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(Skynet.Showroom.Client.IConsultantsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        })
        .AddTypedClient<Skynet.Showroom.Client.IConsultantsClient>((http, sp) => new Skynet.Showroom.Client.ConsultantsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Skynet.Showroom.Client.IOrganizationsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        })
        .AddTypedClient<Skynet.Showroom.Client.IOrganizationsClient>((http, sp) => new Skynet.Showroom.Client.OrganizationsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(Skynet.Showroom.Client.ICompetenceAreasClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        })
        .AddTypedClient<Skynet.Showroom.Client.ICompetenceAreasClient>((http, sp) => new Skynet.Showroom.Client.CompetenceAreasClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        return services;
    }
}