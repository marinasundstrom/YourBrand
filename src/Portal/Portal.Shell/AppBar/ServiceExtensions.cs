using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Portal.AppBar;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppBar(this IServiceCollection services)
    {
        services.AddScoped<IAppBarTrayService, AppBarTrayService>();

        return services;
    }
}