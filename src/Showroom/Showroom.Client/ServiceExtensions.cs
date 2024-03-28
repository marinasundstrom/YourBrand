using Microsoft.Extensions.DependencyInjection;

using YourBrand.Showroom.Client;

namespace YourBrand.Showroom;

public static class ServiceExtensions
{
    public static IServiceCollection AddShowroomClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder> builder)
    {
        builder(
            services.AddHttpClient("Showroom-" + nameof(IPersonProfilesClient), configureClient)
            .AddTypedClient<IPersonProfilesClient>((http, sp) => new PersonProfilesClient(http)));

        builder(
            services.AddHttpClient("Showroom-" + nameof(IOrganizationsClient), configureClient)
            .AddTypedClient<IOrganizationsClient>((http, sp) => new OrganizationsClient(http)));

        builder(
            services.AddHttpClient("Showroom-" + nameof(ICompetenceAreasClient), configureClient)
            .AddTypedClient<ICompetenceAreasClient>((http, sp) => new CompetenceAreasClient(http)));

        builder(
            services.AddHttpClient("Showroom-" + nameof(ISkillsClient), configureClient)
            .AddTypedClient<ISkillsClient>((http, sp) => new SkillsClient(http)));

        builder(
            services.AddHttpClient("Showroom-" + nameof(ISkillAreasClient), configureClient)
            .AddTypedClient<ISkillAreasClient>((http, sp) => new SkillAreasClient(http)));

        builder(
            services.AddHttpClient("Showroom-" + nameof(ICasesClient), configureClient)
            .AddTypedClient<ICasesClient>((http, sp) => new CasesClient(http)));

        builder(
            services.AddHttpClient("Showroom-" + nameof(ICompaniesClient), configureClient)
            .AddTypedClient<ICompaniesClient>((http, sp) => new CompaniesClient(http)));

        builder(
            services.AddHttpClient("Showroom-" + nameof(IIndustriesClient), configureClient)
            .AddTypedClient<IIndustriesClient>((http, sp) => new IndustriesClient(http)));

        return services;
    }
}