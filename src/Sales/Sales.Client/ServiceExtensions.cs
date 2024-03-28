namespace YourBrand.Sales;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddSalesClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        services.AddOrdersClient(configureClient, configureBuilder);

        services.AddOrderStatusesClient(configureClient, configureBuilder);

        services.AddSubscriptionsClient(configureClient, configureBuilder);

        services.AddSubscriptionPlansClient(configureClient, configureBuilder);

        return services;
    }

    public static IServiceCollection AddOrdersClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("SalesAPI", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<IOrdersClient>("SalesAPI")
            .AddTypedClient<IOrdersClient>((http, sp) => new YourBrand.Sales.OrdersClient(http));

        return services;
    }

    public static IServiceCollection AddOrderStatusesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("SalesAPI", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<IOrderStatusesClient>("SalesAPI")
            .AddTypedClient<IOrderStatusesClient>((http, sp) => new YourBrand.Sales.OrderStatusesClient(http));

        return services;
    }

    public static IServiceCollection AddSubscriptionsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("SalesAPI", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<ISubscriptionsClient>("SalesAPI")
            .AddTypedClient<ISubscriptionsClient>((http, sp) => new YourBrand.Sales.SubscriptionsClient(http));

        return services;
    }

    public static IServiceCollection AddSubscriptionPlansClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("SalesAPI", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<ISubscriptionPlansClient>("SalesAPI")
            .AddTypedClient<ISubscriptionPlansClient>((http, sp) => new YourBrand.Sales.SubscriptionPlansClient(http));

        return services;
    }
}