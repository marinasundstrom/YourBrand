using YourBrand.Notifications.Application.Notifications.Queries;

namespace YourBrand.Notifications.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(GetNotificationsQuery)));

        return services;
    }
}