using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Notifications.Client;

public static class ServiceExtensions 
{
    public static IServiceCollection AddNotificationsClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder> builder) 
    {
        builder(
            services.AddHttpClient(nameof(INotificationsClient), configureClient)
            .AddTypedClient<INotificationsClient>((http, sp) => new NotificationsClient(http)));

        return services;
    }
}