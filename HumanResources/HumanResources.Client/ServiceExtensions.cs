using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.HumanResources.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddHumanResourcesClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddUsersClient(configureClient, builder);

        builder(
           services.AddHttpClient(nameof(ITeamsClient) + "TR", configureClient)
           .AddTypedClient<ITeamsClient>((http, sp) => new TeamsClient(http)));

        return services;
    }

    public static IServiceCollection AddUsersClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(UsersClient), configureClient)
            .AddTypedClient<IUsersClient>((http, sp) => new UsersClient(http));

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