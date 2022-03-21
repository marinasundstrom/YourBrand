using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourCompany.Portal.Shared;

namespace YourCompany.Showroom;

public static class ServiceExtensions
{
    public static IServiceCollection AddShowroom(this IServiceCollection services)
    {
        services.AddClients();
        
        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(YourCompany.Showroom.Client.IConsultantsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        })
        .AddTypedClient<YourCompany.Showroom.Client.IConsultantsClient>((http, sp) => new YourCompany.Showroom.Client.ConsultantsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourCompany.Showroom.Client.IOrganizationsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        })
        .AddTypedClient<YourCompany.Showroom.Client.IOrganizationsClient>((http, sp) => new YourCompany.Showroom.Client.OrganizationsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourCompany.Showroom.Client.ICompetenceAreasClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        })
        .AddTypedClient<YourCompany.Showroom.Client.ICompetenceAreasClient>((http, sp) => new YourCompany.Showroom.Client.CompetenceAreasClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        services.AddHttpClient(nameof(YourCompany.Showroom.Client.ISkillsClient), (sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}showroom/");
        })
        .AddTypedClient<YourCompany.Showroom.Client.ISkillsClient>((http, sp) => new YourCompany.Showroom.Client.SkillsClient(http))
        .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        return services;
    }
}