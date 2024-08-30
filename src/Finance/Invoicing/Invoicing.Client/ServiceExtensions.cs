using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Invoicing.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddInvoicingClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddInvoicesClient(configureClient, builder)
            .AddInvoiceStatusesClient(configureClient, builder);

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

    public static IServiceCollection AddInvoiceStatusesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(InvoiceStatusesClient), configureClient)
            .AddTypedClient<IInvoiceStatusesClient>((http, sp) => new InvoiceStatusesClient(http));

        builder?.Invoke(b);

        return services;
    }
}