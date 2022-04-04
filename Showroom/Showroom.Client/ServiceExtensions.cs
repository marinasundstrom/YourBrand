using Microsoft.Extensions.DependencyInjection;

using YourBrand.Showroom.Client;

namespace YourBrand.Showroom;

public static class ServiceExtensions
{
    public static IServiceCollection AddShowroomClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder> builder)
    {
        builder(
            services.AddHttpClient(nameof(IConsultantsClient), configureClient)
            .AddTypedClient<IConsultantsClient>((http, sp) => new ConsultantsClient(http)));

        builder(
            services.AddHttpClient(nameof(IOrganizationsClient), configureClient)
            .AddTypedClient<IOrganizationsClient>((http, sp) => new OrganizationsClient(http)));

        builder(
            services.AddHttpClient(nameof(ICompetenceAreasClient), configureClient)
            .AddTypedClient<ICompetenceAreasClient>((http, sp) => new CompetenceAreasClient(http)));

        builder(
            services.AddHttpClient(nameof(ISkillsClient), configureClient)
            .AddTypedClient<ISkillsClient>((http, sp) => new SkillsClient(http)));

        return services;
    }
}