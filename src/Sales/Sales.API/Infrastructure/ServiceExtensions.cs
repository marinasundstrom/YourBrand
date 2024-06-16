using YourBrand.Domain.Infrastructure;
using YourBrand.Sales.Infrastructure.Services;

namespace YourBrand.Sales.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCoreServices();

        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<IEmailService, EmailService>();

        services.AddDomainInfrastructure(configuration);

        return services;
    }
}