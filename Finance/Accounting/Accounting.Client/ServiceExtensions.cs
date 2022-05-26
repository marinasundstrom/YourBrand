using Microsoft.Extensions.DependencyInjection;

namespace Accounting.Client;

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
            .AddHttpClient(nameof(IVerificationsClient), configureClient)
            .AddTypedClient<IVerificationsClient>((http, sp) => new VerificationsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddEntries(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(IEntriesClient), configureClient)
            .AddTypedClient<IEntriesClient>((http, sp) => new EntriesClient(http));

        builder?.Invoke(b);

        return services;
    }
}