namespace YourBrand.Portal.Modules;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.AppService.Client;

public static class ModuleServiceExtensions
{
    public static async Task ConfigureModuleServices(this IServiceProvider services)
    {
        await services.SetTenantActiveModules();

        var moduleConfigurator = services.GetRequiredService<ModuleConfigurator>();
        await moduleConfigurator.ConfigureServices();
    }

    private static async Task SetTenantActiveModules(this IServiceProvider serviceProvider)
    {
        var tenantModuleClient = serviceProvider.GetRequiredService<ITenantModulesClient>();
        var moduleRegistry = serviceProvider.GetRequiredService<IModuleRegistry>();

        var tenantModules = await tenantModuleClient.GetModulesAsync();

        moduleRegistry.Modules.ToList().ForEach(moduleDescriptor => moduleDescriptor.Enabled = false);

        foreach (var tenantModule in tenantModules)
        {
            var moduleDescriptor = moduleRegistry.Modules.FirstOrDefault(x => x.Id == tenantModule.Module.Id);

            if (moduleDescriptor is null)
                continue;

            moduleDescriptor.Enabled = tenantModule.Enabled;
        }
    }
}
