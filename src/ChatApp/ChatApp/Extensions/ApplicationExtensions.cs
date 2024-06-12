using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Extensions;
using YourBrand.ChatApp.Infrastructure;

namespace YourBrand.ChatApp.Web.Extensions;

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
