using Microsoft.Extensions.DependencyInjection;

using YourBrand.Warehouse.Client;

namespace YourBrand.Warehouse.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddWarehouseClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddItemsClient(configureClient, builder);
            //.AddAddressesClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddItemsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(ItemsClient) + "WH", configureClient)
            .AddTypedClient<IItemsClient>((http, sp) => new ItemsClient(http));

        builder?.Invoke(b);

        return services;
    }
}