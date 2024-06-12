using ChatApp.Domain;
using ChatApp.Extensions;
using ChatApp.Infrastructure;

namespace ChatApp.Web.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddUniverse(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddPresentation()
            .AddApplication()
            .AddDomain()
            .AddInfrastructure(configuration);

        return services;
    }
}
