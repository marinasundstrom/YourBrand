using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.AppService.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServiceClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddItemsClient(configureClient, builder)
            .AddNotificationsClient(configureClient, builder)
            .AddSearchClient(configureClient, builder)
            .AddDoSomethingClient(configureClient, builder)
            .AddWidgetsClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddItemsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(ItemsClient), configureClient)
            .AddTypedClient<IItemsClient>((http, sp) => new ItemsClient(http));

        builder?.Invoke(b);

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

    public static IServiceCollection AddWidgetsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(WidgetsClient), configureClient)
            .AddTypedClient<IWidgetsClient>((http, sp) => new WidgetsClient(http));

        builder?.Invoke(b);

        return services;
    }
}