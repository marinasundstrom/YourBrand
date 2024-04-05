using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Identity;

public static class ServicesExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}