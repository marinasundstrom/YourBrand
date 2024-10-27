namespace YourBrand.Sales;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddSalesClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        services.AddOrdersClient(configureClient, configureBuilder);

        services.AddOrderStatusesClient(configureClient, configureBuilder);

        services.AddOrderTypesClient(configureClient, configureBuilder);

        services.AddSubscriptionsClient(configureClient, configureBuilder);

        services.AddSubscriptionStatusesClient(configureClient, configureBuilder);

        services.AddSubscriptionTypesClient(configureClient, configureBuilder);

        services.AddSubscriptionPlansClient(configureClient, configureBuilder);

        services.AddUsersClient(configureClient, configureBuilder);

        services.AddOrganizationsClient(configureClient, configureBuilder);

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

    public static IServiceCollection AddOrderTypesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("SalesAPI", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<IOrderTypesClient>("SalesAPI")
            .AddTypedClient<IOrderTypesClient>((http, sp) => new YourBrand.Sales.OrderTypesClient(http));

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

    public static IServiceCollection AddSubscriptionStatusesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("SalesAPI", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<ISubscriptionStatusesClient>("SalesAPI")
            .AddTypedClient<ISubscriptionStatusesClient>((http, sp) => new YourBrand.Sales.SubscriptionStatusesClient(http));

        return services;
    }

    public static IServiceCollection AddSubscriptionTypesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("SalesAPI", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<ISubscriptionTypesClient>("SalesAPI")
            .AddTypedClient<ISubscriptionTypesClient>((http, sp) => new YourBrand.Sales.SubscriptionTypesClient(http));

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

    public static IServiceCollection AddUsersClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("SalesAPI", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<IUsersClient>("SalesAPI")
            .AddTypedClient<IUsersClient>((http, sp) => new YourBrand.Sales.UsersClient(http));

        return services;
    }

    public static IServiceCollection AddOrganizationsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("SalesAPI", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<IOrganizationsClient>("SalesAPI")
            .AddTypedClient<IOrganizationsClient>((http, sp) => new YourBrand.Sales.OrganizationsClient(http));

        return services;
    }
}