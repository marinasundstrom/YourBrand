using MediatR;

namespace YourBrand.Payments.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(ServiceCollectionExtensions)));

        return services;
    }
}