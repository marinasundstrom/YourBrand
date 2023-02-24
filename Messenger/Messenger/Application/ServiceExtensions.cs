using YourBrand.Messenger.Application.Messages.Queries;

using MediatR;


namespace YourBrand.Messenger.Application;

public static class ServiceExtensions
{
    private const string ApiKey = "asdsr34#34rswert35234aedae?2!";

    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(GetMessagesIncrQuery)));

        /*
        services.AddHttpClient(nameof(INotificationsClient), (sp, http) =>
        {
            var conf = sp.GetRequiredService<IConfiguration>();

            http.BaseAddress = conf.GetServiceUri("notifications");
            http.DefaultRequestHeaders.Add("X-API-KEY", ApiKey);
        })
        .AddTypedClient<INotificationsClient>((http, sp) => new NotificationsClient(http))
        .AddHttpMessageHandler<Handler>();
        */

        return services;
    }
}