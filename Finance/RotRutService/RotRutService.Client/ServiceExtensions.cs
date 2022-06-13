using Microsoft.Extensions.DependencyInjection;

using YourBrand.RotRutService.Client;

namespace YourBrand.RotRutService.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddRotRutClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddRotRutClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddRotRutClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(IClient), configureClient)
            .AddTypedClient<IClient>((http, sp) => new Client(http));

        builder?.Invoke(b);

        return services;
    }
}