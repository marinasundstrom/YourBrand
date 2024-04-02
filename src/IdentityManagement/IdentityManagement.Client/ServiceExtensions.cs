using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.IdentityManagement.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddIdentityManagementClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddOrganizationsClient(configureClient, builder)
            .AddUsersClient(configureClient, builder)
            .AddRolesClient(configureClient, builder)
            .AddSyncClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddOrganizationsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(OrganizationsClient) + "IS", configureClient)
            .AddTypedClient<IOrganizationsClient>((http, sp) => new OrganizationsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddUsersClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(UsersClient) + "IS", configureClient)
            .AddTypedClient<IUsersClient>((http, sp) => new UsersClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddRolesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(RolesClient) + "IS", configureClient)
            .AddTypedClient<IRolesClient>((http, sp) => new RolesClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddSyncClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(SyncClient) + "IS", configureClient)
            .AddTypedClient<ISyncClient>((http, sp) => new SyncClient(http));

        builder?.Invoke(b);

        return services;
    }
}