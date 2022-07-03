using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.HumanResources.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddHumanResourcesClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddPersonsClient(configureClient, builder)
            .AddRolesClient(configureClient, builder);

        builder?.Invoke(
           services.AddHttpClient(nameof(ITeamsClient) + "HR", configureClient)
           .AddTypedClient<ITeamsClient>((http, sp) => new TeamsClient(http)));

        return services;
    }

    public static IServiceCollection AddPersonsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(PersonsClient), configureClient)
            .AddTypedClient<IPersonsClient>((http, sp) => new PersonsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddRolesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(RolesClient), configureClient)
            .AddTypedClient<IRolesClient>((http, sp) => new RolesClient(http));

        builder?.Invoke(b);

        return services;
    }
}