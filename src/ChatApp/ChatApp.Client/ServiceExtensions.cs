using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.ChatApp.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddChatAppClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder> builder)
    {
        builder(
            services.AddHttpClient(nameof(IChannelsClient), configureClient)
            .AddTypedClient<IChannelsClient>((http, sp) => new ChannelsClient(http)));

        builder(
            services.AddHttpClient(nameof(IMessagesClient), configureClient)
            .AddTypedClient<IMessagesClient>((http, sp) => new MessagesClient(http)));

        builder(
            services.AddHttpClient(nameof(IUsersClient), configureClient)
            .AddTypedClient<IUsersClient>((http, sp) => new UsersClient(http)));

        return services;
    }

    public static IServiceCollection AddEmojiService(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder> builder)
    {
        builder(
        services.AddHttpClient(nameof(MudEmojiPicker.Data.EmojiService), configureClient)
        .AddTypedClient<MudEmojiPicker.Data.EmojiService>((http, sp) => new MudEmojiPicker.Data.EmojiService(http)));

        return services;
    }
}