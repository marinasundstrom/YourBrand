using Microsoft.Extensions.DependencyInjection;

namespace Transactions.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddTransactionsClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddTransactionsClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddTransactionsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(TransactionsClient), configureClient)
            .AddTypedClient<ITransactionsClient>((http, sp) => new TransactionsClient(http));

        builder?.Invoke(b);

        return services;
    }
}