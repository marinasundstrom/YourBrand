using Microsoft.Extensions.Logging;

namespace YourBrand.Portal.Modules;

public class ModuleConfigurator(IServiceProvider serviceProvider, IModuleRegistry moduleRegistry, ILogger<ModuleConfigurator> logger)
{
    private static readonly Type _moduleInitializerInterface = typeof(IModuleInitializer);

    public async Task ConfigureServices()
    {
        foreach (var module in moduleRegistry.GetEnabledModules())
        {
            var initializerType = module.GetInitializerType();

            if (initializerType is not null)
            {
                var moduleInitializeMethod = initializerType.GetMethod("ConfigureServices");
                await (Task)moduleInitializeMethod?.Invoke(null, [serviceProvider])!;

                logger.LogInformation($"Module \"{module.Assembly.GetName().Name}\" was configured.");
            }
        }
    }
}
