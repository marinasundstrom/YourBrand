using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.ApiKeys.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddApiKeyClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(IApiKeysClient), configureClient)
            .AddTypedClient<IApiKeysClient>((http, sp) => new ApiKeysClient(http));

        builder?.Invoke(b);

        return services;
    }
}