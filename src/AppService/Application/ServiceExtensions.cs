using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Notifications.Client;

namespace YourBrand.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(ServiceExtensions)));

        services.AddScoped<AuthForwardHandler>();

        services.AddNotificationsClients((sp, http) =>
        {
            http.BaseAddress = new Uri("https://localhost:5030/");
        },
        builder =>
        {
            builder.AddHttpMessageHandler<AuthForwardHandler>();
        });

        return services;
    }
}
