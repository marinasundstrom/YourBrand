using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.IdentityManagement.Application.Users.Commands;

namespace YourBrand.IdentityManagement.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(CreateUserCommand)));

        return services;
    }
}