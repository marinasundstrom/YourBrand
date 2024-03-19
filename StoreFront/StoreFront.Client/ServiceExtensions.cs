namespace YourBrand.StoreFront;

using Microsoft.Extensions.DependencyInjection;
using BlazorApp;
using System.Text.Json;
using Microsoft.JSInterop;

public static class ServiceExtensions
{
    public static IServiceCollection AddStoreFrontClients(this IServiceCollection services, Uri baseUrl, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        services.AddCatalogClients((sp, http) =>
        {
            http.BaseAddress = baseUrl;
        }, configureBuilder);

        services.AddCartClient((sp, http) =>
        {
            http.BaseAddress = baseUrl;
        }, configureBuilder);

        services.AddCheckoutClient((sp, http) =>
        {
            http.BaseAddress = baseUrl;
        }, configureBuilder);

        services.AddBrandsClient((sp, http) =>
                {
                    http.BaseAddress = baseUrl;
                }, configureBuilder);

        services.AddAnalyticsClient((sp, http) =>
            {
                http.BaseAddress = baseUrl;
            }, configureBuilder);

        return services;
    }

    public static IServiceCollection AddStoreFrontClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        services.AddCatalogClients(configureClient, configureBuilder);

        services.AddCartClient(configureClient, configureBuilder);

        services.AddCheckoutClient(configureClient, configureBuilder);

        services.AddBrandsClient(configureClient, configureBuilder);

        services.AddAnalyticsClient(configureClient, configureBuilder);

        return services;
    }

    public static IServiceCollection AddCatalogClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("StoreFront", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<IProductsClient>("StoreFront")
            .AddTypedClient<IProductsClient>((http, sp) => new YourBrand.StoreFront.ProductsClient(http));

        services.AddHttpClient<IProductCategoriesClient>("StoreFront")
            .AddTypedClient<IProductCategoriesClient>((http, sp) => new YourBrand.StoreFront.ProductCategoriesClient(http));

        return services;
    }

    public static IServiceCollection AddCartClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("StoreFront", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<ICartClient>("StoreFront")
            .AddTypedClient<ICartClient>((http, sp) =>
            {
                var renderingContext = sp.GetRequiredService<RenderingContext>();
                if (!renderingContext.IsPrerendering)
                {
                    var clientId = JsonSerializer.Deserialize<string>(
                        sp.GetRequiredService<IJSInProcessRuntime>().Invoke<string>("getCid"));

                    http.DefaultRequestHeaders.Add("X-Client-Id", clientId);
                }
                return new CartClient(http);
            });

        return services;
    }

    public static IServiceCollection AddCheckoutClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("StoreFront", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<ICheckoutClient>("StoreFront")
            .AddTypedClient<ICheckoutClient>((http, sp) => new YourBrand.StoreFront.CheckoutClient(http));

        return services;
    }

    public static IServiceCollection AddBrandsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("StoreFront", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<IBrandsClient>("StoreFront")
            .AddTypedClient<IBrandsClient>((http, sp) => new YourBrand.StoreFront.BrandsClient(http));

        return services;
    }

    public static IServiceCollection AddAnalyticsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("StoreFront", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<IAnalyticsClient>("StoreFront")
            .AddTypedClient<IAnalyticsClient>((http, sp) => new YourBrand.StoreFront.AnalyticsClient(http));

        return services;
    }
}