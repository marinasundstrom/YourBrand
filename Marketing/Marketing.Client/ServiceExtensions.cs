using Microsoft.Extensions.DependencyInjection;

using YourBrand.Marketing.Client;

namespace YourBrand.Marketing.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddMarketingClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddMarketingClient(configureClient, builder)
            .AddProspectsClient(configureClient, builder);
            //.AddAddressesClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddProspectsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(ProspectsClient) + "M", configureClient)
            .AddTypedClient<IProspectsClient>((http, sp) => new ProspectsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddMarketingClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        /*
        var b = services
            .AddHttpClient(nameof(MarketingClient) + "M", configureClient)
            .AddTypedClient<IMarketingClient>((http, sp) => new MarketingClient(http));

        builder?.Invoke(b);
        */

        return services;
    }

    /*

    public static IServiceCollection AddAddressesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(AddressesClient), configureClient)
            .AddTypedClient<IAddressesClient>((http, sp) => new AddressesClient(http));

        builder?.Invoke(b);

        return services;
    }

    */
}