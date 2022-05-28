using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Payments.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddPaymentsClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddPaymentsClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddPaymentsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        /*
        var b = services
            .AddHttpClient(nameof(PaymentsClient), configureClient)
            .AddTypedClient<IPaymentsClient>((http, sp) => new PaymentsClient(http));

        */

        //builder?.Invoke(b);

        return services;
    }
}