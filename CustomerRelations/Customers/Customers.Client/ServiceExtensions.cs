using Microsoft.Extensions.DependencyInjection;

using YourBrand.Customers.Client;

namespace YourBrand.Customers.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomersClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddCustomersClient(configureClient, builder)
            .AddPersonsClient(configureClient, builder)
            .AddOrganizationsClient(configureClient, builder);
            //.AddAddressesClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddPersonsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(PersonsClient) + "HR", configureClient)
            .AddTypedClient<IPersonsClient>((http, sp) => new PersonsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddOrganizationsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(OrganizationsClient) + "HR", configureClient)
            .AddTypedClient<IOrganizationsClient>((http, sp) => new OrganizationsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddCustomersClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(CustomersClient) + "HR", configureClient)
            .AddTypedClient<ICustomersClient>((http, sp) => new CustomersClient(http));

        builder?.Invoke(b);

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