using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Meetings.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddMeetingsClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddMeetingsClient(configureClient, builder)
            .AddUsersClient(configureClient, builder);

        return services;
    }
    public static IServiceCollection AddMeetingsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(MeetingsClient) + "TS", configureClient)
            .AddTypedClient<IMeetingsClient>((http, sp) => new MeetingsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddUsersClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(UsersClient) + "TS", configureClient)
            .AddTypedClient<IUsersClient>((http, sp) => new UsersClient(http));

        builder?.Invoke(b);

        return services;
    }
}