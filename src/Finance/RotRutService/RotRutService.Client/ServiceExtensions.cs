using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.RotRutService.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddRotRutClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddRotRutClient(configureClient, builder)
            .AddCasesClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddRotRutClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(IRotRutServiceClient), configureClient)
            .AddTypedClient<IRotRutServiceClient>((http, sp) => new RotRutServiceClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddCasesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(ICasesClient), configureClient)
            .AddTypedClient<ICasesClient>((http, sp) => new CasesClient(http));

        builder?.Invoke(b);

        return services;
    }
}