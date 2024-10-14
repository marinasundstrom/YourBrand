using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Meetings.Client;

public static class ServiceExtensions
{
    public static IServiceCollection AddMeetingsClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        services
            .AddMeetingsClient(configureClient, builder)
            .AddAgendasClient(configureClient, builder)
            .AddMotionsClient(configureClient, builder)
            .AddMinutesClient(configureClient, builder)
            .AddMeetingGroupsClient(configureClient, builder)
            .AddUsersClient(configureClient, builder);

        return services;
    }
    public static IServiceCollection AddMeetingsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(MeetingsClient) + "MS", configureClient)
            .AddTypedClient<IMeetingsClient>((http, sp) => new MeetingsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddAgendasClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(AgendasClient) + "MS", configureClient)
            .AddTypedClient<IAgendasClient>((http, sp) => new AgendasClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddMotionsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(MotionsClient) + "MS", configureClient)
            .AddTypedClient<IMotionsClient>((http, sp) => new MotionsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddMinutesClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(MinutesClient) + "MS", configureClient)
            .AddTypedClient<IMinutesClient>((http, sp) => new MinutesClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddMeetingGroupsClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(MeetingGroupsClient) + "MS", configureClient)
            .AddTypedClient<IMeetingGroupsClient>((http, sp) => new MeetingGroupsClient(http));

        builder?.Invoke(b);

        return services;
    }

    public static IServiceCollection AddUsersClient(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? builder = null)
    {
        var b = services
            .AddHttpClient(nameof(UsersClient) + "MS", configureClient)
            .AddTypedClient<IUsersClient>((http, sp) => new UsersClient(http));

        builder?.Invoke(b);

        return services;
    }
}