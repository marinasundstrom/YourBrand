using BlazorApp.Cart;

using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Cart;

public static class ServiceExtensions
{
    public static IServiceCollection AddCartServices(this IServiceCollection services)
    {
        services.AddSingleton<ICartService, CartService>();

        return services;
    }
}