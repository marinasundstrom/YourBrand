using Microsoft.Extensions.DependencyInjection;

namespace Invoices.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddInvoicesClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddInvoicesClient(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddInvoicesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(InvoicesClient), configureClient)
            .AddTypedClient<IInvoicesClient>((http, sp) => new InvoicesClient(http));

        builder?.Invoke(b);

        return services;
    }
}