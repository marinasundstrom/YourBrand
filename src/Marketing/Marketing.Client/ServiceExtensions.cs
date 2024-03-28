using Microsoft.Extensions.DependencyInjection;

using YourBrand.Marketing.Client;

namespace YourBrand.Marketing.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddMarketingClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddMarketingClient(configureClient, builder)
            .AddContactsClient(configureClient, builder)
            .AddCampaignsClient(configureClient, builder)
            .AddDiscountsClient(configureClient, builder);
            //.AddAddressesClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddContactsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(ContactsClient) + "M", configureClient)
            .AddTypedClient<IContactsClient>((http, sp) => new ContactsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddCampaignsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(CampaignsClient) + "M", configureClient)
            .AddTypedClient<ICampaignsClient>((http, sp) => new CampaignsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddDiscountsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(DiscountsClient) + "M", configureClient)
            .AddTypedClient<IDiscountsClient>((http, sp) => new DiscountsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddMarketingClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        /*
        var b = services
            .AddHttpClient(nameof(MarketingClient) + "M", configureClient)
            .AddTypedClient<IMarketingClient>((http, sp) => new MarketingClient(http));

        builder?.Invoke(b);
        */

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