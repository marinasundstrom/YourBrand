using Microsoft.Extensions.DependencyInjection;

using YourBrand.Customers.Client;

namespace YourBrand.Customers.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomersClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddPersonsClient(configureClient, builder);
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