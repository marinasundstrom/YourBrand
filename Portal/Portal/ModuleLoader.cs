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
                new Module(typeof(YourBrand.TimeReport.ModuleInitializer).Assembly),
                new Module(typeof(YourBrand.Showroom.ExampleJsInterop).Assembly) { Enabled = false },
                new Module(typeof(YourBrand.Accounting.ServiceExtensions).Assembly) { Enabled = false },
                new Module(typeof(YourBrand.Invoices.ServiceExtensions).Assembly) { Enabled = false },
                new Module(typeof(YourBrand.Transactions.ServiceExtensions).Assembly) { Enabled = false },
                new Module(typeof(YourBrand.Documents.ServiceExtensions).Assembly) { Enabled = false },
                new Module(typeof(YourBrand.Messenger.ServiceExtensions).Assembly) { Enabled = false },
                new Module(typeof(YourBrand.RotRutService.ServiceExtensions).Assembly) { Enabled = false },
                new Module(typeof(YourBrand.Customers.ServiceExtensions).Assembly) { Enabled = false },
                new Module(typeof(YourBrand.HumanResources.ModuleInitializer).Assembly)
        };

    private readonly IServiceProvider _serviceProvider;
    private static Type _moduleInitializerInterface;

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

        services
            //.AddTimeReport()
            .AddShowroom()
            .AddAccounting()
            .AddInvoicing()
            .AddPayments()
            .AddTransactions()
            .AddDocuments()
            .AddMessenger()
            .AddRotAndRut()
            .AddCustomers();
            //.AddHumanResources();
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