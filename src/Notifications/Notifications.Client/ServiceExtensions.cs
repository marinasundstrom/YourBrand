using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Notifications.Client;

public static class ServiceExtensions
{
    private static bool registered;

    public static IServiceCollection AddNotificationsClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder> builder)
    {
        if(registered)
           return services;

            builder(
                services.AddHttpClient(nameof(INotificationsClient), configureClient)
                .AddTypedClient<INotificationsClient>((http, sp) => new NotificationsClient(http)));

            registered = true;

        return services;
    }
}