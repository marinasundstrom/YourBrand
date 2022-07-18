using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using YourBrand.Portal;
using YourBrand.Portal.Theming;
using Blazored.LocalStorage;

using YourBrand.Portal.Navigation;
using YourBrand.Portal.Modules;
using System.Reflection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Local", options.ProviderOptions);

    options.UserOptions.NameClaim = "name";
    options.UserOptions.RoleClaim = "role";
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage();

builder.Services
    .AddServices()
    .AddThemeServices()
    .AddNavigationServices()
    .AddScoped<ModuleLoader>();

LoadModules(builder.Services);

var app = builder.Build();

await app.Services.Localize();

var moduleBuilder = app.Services.GetRequiredService<ModuleLoader>();
moduleBuilder.ConfigureServices();

await app.RunAsync();

void LoadModules(IServiceCollection services)
{
    var moduleAssemblies = new List<ModuleEntry>
    {
        new ModuleEntry(typeof(YourBrand.Showroom.ModuleInitializer).Assembly, false),
        new ModuleEntry(typeof(YourBrand.Catalog.ModuleInitializer).Assembly, true),
        new ModuleEntry(typeof(YourBrand.Orders.ModuleInitializer).Assembly, true),
        new ModuleEntry(typeof(YourBrand.Marketing.ModuleInitializer).Assembly, false),
        new ModuleEntry(typeof(YourBrand.TimeReport.ModuleInitializer).Assembly, false),
        new ModuleEntry(typeof(YourBrand.Invoicing.ModuleInitializer).Assembly, false),
        new ModuleEntry(typeof(YourBrand.Transactions.ModuleInitializer).Assembly, false),
        new ModuleEntry(typeof(YourBrand.Payments.ModuleInitializer).Assembly, false),
        new ModuleEntry(typeof(YourBrand.Accounting.ModuleInitializer).Assembly, false),
        new ModuleEntry(typeof(YourBrand.Documents.ModuleInitializer).Assembly, false),
        new ModuleEntry(typeof(YourBrand.Messenger.ModuleInitializer).Assembly, false),
        new ModuleEntry(typeof(YourBrand.RotRutService.ModuleInitializer).Assembly, false),
        new ModuleEntry(typeof(YourBrand.Customers.ModuleInitializer).Assembly, false),
        new ModuleEntry(typeof(YourBrand.HumanResources.ModuleInitializer).Assembly, true),
        new ModuleEntry(typeof(YourBrand.Warehouse.ModuleInitializer).Assembly, false)
    };

    moduleAssemblies.ForEach(x => ModuleLoader.LoadModule(x.Assembly, x.Enabled));

    ModuleLoader.AddServices(builder.Services);
}

record ModuleEntry(Assembly Assembly, bool Enabled);
