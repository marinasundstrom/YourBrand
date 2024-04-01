using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Accounting.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddAccountingClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddAccounts(configureClient, builder)
            .AddVerifications(configureClient, builder)
            .AddEntries(configureClient, builder);

        return services;
    }

    public static IServiceCollection AddAccounts(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(IAccountsClient), configureClient)
            .AddTypedClient<IAccountsClient>((http, sp) => new AccountsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddVerifications(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(IJournalEntriesClient), configureClient)
            .AddTypedClient<IJournalEntriesClient>((http, sp) => new JournalEntriesClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddEntries(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(ILedgerEntriesClient), configureClient)
            .AddTypedClient<ILedgerEntriesClient>((http, sp) => new LedgerEntriesClient(http));

        builder?.Invoke(b);

        return services;
    }
}