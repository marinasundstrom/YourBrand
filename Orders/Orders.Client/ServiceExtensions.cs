using Microsoft.Extensions.DependencyInjection;

using YourBrand.Orders.Client;

namespace YourBrand.Orders.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddOrdersClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddOrdersClient(configureClient, builder)
            .AddOrderStatusesClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddOrdersClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(OrdersClient), configureClient)
            .AddTypedClient<IOrdersClient>((http, sp) => new OrdersClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddOrderStatusesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(OrderStatusesClient), configureClient)
            .AddTypedClient<IOrderStatusesClient>((http, sp) => new OrderStatusesClient(http));

        builder?.Invoke(b);

        return services;
    }
}
