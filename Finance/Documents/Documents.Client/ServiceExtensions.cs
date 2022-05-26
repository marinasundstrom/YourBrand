using Microsoft.Extensions.DependencyInjection;

namespace Documents.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddDocumentsClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddDocumentsClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddDocumentsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(DocumentsClient), configureClient)
            .AddTypedClient<IDocumentsClient>((http, sp) => new DocumentsClient(http));

        builder?.Invoke(b);

        return services;
    }
}