namespace YourBrand.StoreFront.API.Features.Cart;

public static class ServiceExtensions
{
    public static IServiceCollection AddCartServices(this IServiceCollection services)
    {
        services.AddScoped<MassTransitCartsClient>();

        return services;
    }
}