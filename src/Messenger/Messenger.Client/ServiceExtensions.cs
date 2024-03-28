using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Messenger.Client;

public static class ServiceExtensions 
{
    public static IServiceCollection AddMessengerClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder> builder) 
    {
        builder(
            services.AddHttpClient(nameof(IConversationsClient), configureClient)
            .AddTypedClient<IConversationsClient>((http, sp) => new ConversationsClient(http)));

        return services;
    }
}