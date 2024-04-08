using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Identity;

public static class ServicesExtensions
{
    public static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        services.AddScoped<ISettableUserContext, UserContext>();
        services.AddScoped<IUserContext>(sp => sp.GetRequiredService<ISettableUserContext>());

        return services;
    }
}