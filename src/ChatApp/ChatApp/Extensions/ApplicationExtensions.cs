using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Infrastructure;
using YourBrand.Extensions;

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