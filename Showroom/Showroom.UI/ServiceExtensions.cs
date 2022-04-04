using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;

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
        services.AddHttpClient(nameof(YourBrand.Showroom.Client.IConsultantsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        })
        .AddTypedClient<YourBrand.Showroom.Client.IConsultantsClient>((http, sp) => new YourBrand.Showroom.Client.ConsultantsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourBrand.Showroom.Client.IOrganizationsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        })
        .AddTypedClient<YourBrand.Showroom.Client.IOrganizationsClient>((http, sp) => new YourBrand.Showroom.Client.OrganizationsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourBrand.Showroom.Client.ICompetenceAreasClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        })
        .AddTypedClient<YourBrand.Showroom.Client.ICompetenceAreasClient>((http, sp) => new YourBrand.Showroom.Client.CompetenceAreasClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourBrand.Showroom.Client.ISkillsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        })
        .AddTypedClient<YourBrand.Showroom.Client.ISkillsClient>((http, sp) => new YourBrand.Showroom.Client.SkillsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        return services;
    }
}