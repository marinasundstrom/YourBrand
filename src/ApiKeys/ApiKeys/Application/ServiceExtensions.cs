using YourBrand.ApiKeys.Application.Commands;

namespace YourBrand.ApiKeys.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(CheckApiKeyCommand)));

        return services;
    }
}