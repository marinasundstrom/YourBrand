using Microsoft.Extensions.DependencyInjection;

using YourBrand.Analytics.Client;

namespace YourBrand.Analytics.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddAnalyticsClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddClientClient(configureClient, builder)
            .AddSessionClient(configureClient, builder)
            .AddEventsClient(configureClient, builder)
            .AddStatisticsClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddEventsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(EventsClient) + "A", configureClient)
            .AddTypedClient<IEventsClient>((http, sp) => new EventsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddClientClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(ClientClient) + "A", configureClient)
            .AddTypedClient<IClientClient>((http, sp) => new ClientClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddSessionClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(SessionClient) + "A", configureClient)
            .AddTypedClient<ISessionClient>((http, sp) => new SessionClient(http));

        builder?.Invoke(b);

        return services;
    }
    public static IServiceCollection AddStatisticsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(StatisticsClient) + "A", configureClient)
            .AddTypedClient<IStatisticsClient>((http, sp) => new StatisticsClient(http));

        builder?.Invoke(b);

        return services;
    }
}