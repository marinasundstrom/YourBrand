using System;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Portal.Modules;

public class ModuleLoader
{
    static readonly List<Module> _modules = new List<Module>();

    private readonly IServiceProvider _serviceProvider;
    private static readonly Type _moduleInitializerInterface;

    public IReadOnlyList<Module> Modules => _modules;

    public IEnumerable<Assembly> GetAssemblies() => _modules
        .Where(x => x.Enabled)
        .Select(x => x.Assembly);

    public ModuleLoader(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    static ModuleLoader() 
    {
        _moduleInitializerInterface = typeof(IModuleInitializer);
    }

    public static void LoadModule(string name, Assembly assembly, bool enabled) => _modules.Add(new Module(name, assembly) { Enabled = enabled });

    public static void AddServices(IServiceCollection services)
    {
        foreach (var module in _modules
            .Where(x => x.Enabled))
        {
            var initializer = module.Assembly
                .GetTypes()
                .FirstOrDefault(x => x.GetInterfaces().Any(x => x == _moduleInitializerInterface));

            if (initializer is not null)
            {
                var moduleInitializeMethod = initializer.GetMethod("Initialize");
                moduleInitializeMethod?.Invoke(null, new object[] { services });

                Console.WriteLine($"Module \"{module.Name}\" was initialized.");
            }
        }
    }

    public void ConfigureServices()
    {
        foreach (var module in _modules
            .Where(x => x.Enabled))
        {
            var initializer = module.Assembly
                .GetTypes()
                .FirstOrDefault(x => x.GetInterfaces().Any(x => x == _moduleInitializerInterface));

            if (initializer is not null)
            {
                var moduleInitializeMethod = initializer.GetMethod("ConfigureServices");
                moduleInitializeMethod?.Invoke(null, new object[] { _serviceProvider });

                Console.WriteLine($"Module \"{module.Assembly.GetName().Name}\" was configured.");
            }
        }
    }
}