using System;
using System.Reflection;

using YourBrand.Accounting;
using YourBrand.Customers;
using YourBrand.Documents;
using YourBrand.HumanResources;
using YourBrand.Invoices;
using YourBrand.Messenger;
using YourBrand.Payments;
using YourBrand.Portal.Modules;
using YourBrand.RotRutService;
using YourBrand.Showroom;
using YourBrand.TimeReport;
using YourBrand.Transactions;

namespace YourBrand.Portal;

public class ModuleLoader
{
    static readonly Module[] _modules = new Module[] {
                new Module(typeof(YourBrand.TimeReport.ModuleInitializer).Assembly) { Enabled = true },
                new Module(typeof(YourBrand.Showroom.ModuleInitializer).Assembly) { Enabled = true },
                new Module(typeof(YourBrand.Accounting.ModuleInitializer).Assembly) { Enabled = true },
                new Module(typeof(YourBrand.Invoices.ModuleInitializer).Assembly) { Enabled = true },
                new Module(typeof(YourBrand.Transactions.ModuleInitializer).Assembly) { Enabled = true },
                new Module(typeof(YourBrand.Documents.ModuleInitializer).Assembly) { Enabled = true },
                new Module(typeof(YourBrand.Messenger.ModuleInitializer).Assembly) { Enabled = true },
                new Module(typeof(YourBrand.RotRutService.ModuleInitializer).Assembly) { Enabled = true },
                new Module(typeof(YourBrand.Customers.ModuleInitializer).Assembly) { Enabled = true },
                new Module(typeof(YourBrand.HumanResources.ModuleInitializer).Assembly) { Enabled = true }
        };

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

    public static void AddServices(IServiceCollection services)
    {
        foreach (var module in _modules.Where(x => x.Enabled))
        {
            var initializer = module.Assembly
                .GetTypes()
                .FirstOrDefault(x => x.GetInterfaces().Any(x => x == _moduleInitializerInterface));

            if (initializer is not null)
            {
                var moduleInitializeMethod = initializer.GetMethod("Initialize");
                moduleInitializeMethod?.Invoke(null, new object[] { services });

                Console.WriteLine($"Module \"{module.Assembly.GetName().Name}\" was initialized.");
            }
        }
    }

    public void ConfigureServices()
    {
        foreach (var module in _modules.Where(x => x.Enabled))
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

        //_serviceProvider
            //.UseHumanResources();
            //.UseTimeReport();
    }
}