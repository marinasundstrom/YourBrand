namespace YourBrand.Portal.Modules;

using Microsoft.Extensions.DependencyInjection;

public static class ModuleServiceCollectionExtensions
{
    public static IServiceCollection AddModuleServices(this IServiceCollection services)
    {
        services.AddScoped<ModuleConfigurator>();
        return services;
    }
}
