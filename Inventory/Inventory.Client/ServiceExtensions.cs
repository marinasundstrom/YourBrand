using Microsoft.Extensions.DependencyInjection;

using YourBrand.Inventory.Client;

namespace YourBrand.Inventory.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddInventoryClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddItemsClient(configureClient, builder)
            .AddItemGroupsClient(configureClient, builder)
            .AddSitesClient(configureClient, builder)
            .AddWarehousesClient(configureClient, builder)
            .AddWarehouseItemsClient(configureClient, builder);

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

    public static IServiceCollection AddItemGroupsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(GroupsClient) + "WH", configureClient)
            .AddTypedClient<IGroupsClient>((http, sp) => new GroupsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddSitesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(SitesClient) + "WH", configureClient)
            .AddTypedClient<ISitesClient>((http, sp) => new SitesClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddWarehousesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(WarehousesClient) + "WH", configureClient)
            .AddTypedClient<IWarehousesClient>((http, sp) => new WarehousesClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddWarehouseItemsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(WarehouseItemsClient) + "WH", configureClient)
            .AddTypedClient<IWarehouseItemsClient>((http, sp) => new WarehouseItemsClient(http));

        builder?.Invoke(b);

        return services;
    }
}