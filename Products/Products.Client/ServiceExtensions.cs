using Microsoft.Extensions.DependencyInjection;

using YourBrand.Products.Client;

namespace YourBrand.Products.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddProductsClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddProductsClient(configureClient, builder)
            .AddOptionsClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddProductsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
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
}