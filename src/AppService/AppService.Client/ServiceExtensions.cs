using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.AppService.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServiceClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddNotificationsClient(configureClient, builder)
            .AddSearchClient(configureClient, builder)
            .AddModulesClient(configureClient, builder)
            .AddDoSomethingClient(configureClient, builder)
            .AddWidgetsClient(configureClient, builder)
            .AddOrganizationsClient(configureClient, builder)
            .AddBrandProfilesClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddNotificationsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(NotificationsClient), configureClient)
            .AddTypedClient<INotificationsClient>((http, sp) => new NotificationsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddSearchClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(SearchClient), configureClient)
            .AddTypedClient<ISearchClient>((http, sp) => new SearchClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddDoSomethingClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(DoSomethingClient), configureClient)
            .AddTypedClient<IDoSomethingClient>((http, sp) => new DoSomethingClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddSetupClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(SetupClient), configureClient)
            .AddTypedClient<ISetupClient>((http, sp) => new SetupClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddModulesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(ModulesClient), configureClient)
            .AddTypedClient<IModulesClient>((http, sp) => new ModulesClient(http));

        builder?.Invoke(b);

        var b2 = services
            .AddHttpClient(nameof(TenantModulesClient), configureClient)
            .AddTypedClient<ITenantModulesClient>((http, sp) => new TenantModulesClient(http));

        builder?.Invoke(b2);

        return services;
    }


    public static IServiceCollection AddWidgetsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(WidgetsClient), configureClient)
            .AddTypedClient<IWidgetsClient>((http, sp) => new WidgetsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddOrganizationsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(OrganizationsClient), configureClient)
            .AddTypedClient<IOrganizationsClient>((http, sp) => new OrganizationsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddBrandProfilesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(BrandProfileClient), configureClient)
            .AddTypedClient<IBrandProfileClient>((http, sp) => new BrandProfileClient(http));

        builder?.Invoke(b);

        return services;
    }
}