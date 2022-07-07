using Microsoft.Extensions.DependencyInjection;

using YourBrand.Catalog.Client;

namespace YourBrand.Catalog.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddCatalogClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddCatalogClient(configureClient, builder)
            .AddOptionsClient(configureClient, builder)
            .AddAttributesClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddCatalogClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(ProductsClient), configureClient)
            .AddTypedClient<IProductsClient>((http, sp) => new ProductsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddOptionsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(OptionsClient), configureClient)
            .AddTypedClient<IOptionsClient>((http, sp) => new OptionsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddAttributesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(AttributesClient), configureClient)
            .AddTypedClient<IAttributesClient>((http, sp) => new AttributesClient(http));

        builder?.Invoke(b);

        return services;
    }
}