using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Portal.Modules;

public interface IModuleInitializer
{
    static abstract void Initialize(IServiceCollection services);

    static abstract Task ConfigureServices(IServiceProvider services);
}