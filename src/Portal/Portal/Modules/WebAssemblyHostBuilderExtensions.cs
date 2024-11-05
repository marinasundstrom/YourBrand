using System.Reflection;
using System.Reflection.Metadata;

using YourBrand.AppService.Client;

namespace YourBrand.Portal.Modules;

using System.Diagnostics.Contracts;
using System.Reflection;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

public static class WebAssemblyHostBuilderExtensions
{
    public static async Task LoadModules(this WebAssemblyHostBuilder builder)
    {
        try
        {
            using (var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/appservice/")
            })
            {
                var modulesClient = new ModulesClient(httpClient);

                // Fetch modules from an external source via modulesClient
                var moduleEntries = await modulesClient.GetModulesAsync();

                var moduleRegistry = new ModuleRegistry();
                builder.Services.AddScoped<IModuleRegistry>(sp => moduleRegistry);

                // Register each enabled module in ModuleRegistry
                foreach (var module in moduleEntries.Where(x => x.Enabled))
                {
                    var moduleDescriptor = moduleRegistry.RegisterModule(module.Id, module.Name, Assembly.Load(module.Assembly), module.Enabled);

                    Console.WriteLine($"Module \"{moduleDescriptor.Name}\" was loaded.");
                }

                InitializeModules(moduleRegistry, builder.Services);
            }
        }
        catch (Exception exc)
        {
            Console.WriteLine($"An error occurred while loading modules: {exc.Message}");
        }
    }

    private static readonly Type _moduleInitializerInterface = typeof(IModuleInitializer);

    private static void InitializeModules(ModuleRegistry moduleRegistry, IServiceCollection services)
    {
        foreach (var moduleDescriptor in moduleRegistry.GetEnabledModules())
        {
            Type? initializer = moduleDescriptor.GetInitializerType();

            if (initializer is null)
            {
                Console.WriteLine($"No initializer found for module \"{moduleDescriptor.Name}\".");

                continue;
            }

            var moduleInitializeMethod = initializer.GetMethod("Initialize");
            moduleInitializeMethod?.Invoke(null, [services]);

            Console.WriteLine($"Module \"{moduleDescriptor.Name}\" was initialized.");
        }
    }
}
