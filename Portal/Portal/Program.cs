using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using YourBrand.Accounting;
using YourBrand.Invoices;
using YourBrand.Payments;
using YourBrand.Portal;
using YourBrand.Showroom;
using YourBrand.TimeReport;
using YourBrand.Transactions;
using YourBrand.Portal.Theming;
using Blazored.LocalStorage;
using YourBrand.Documents;
using YourBrand.Messenger;
using YourBrand.RotRutService;
using YourBrand.Customers;
using YourBrand.HumanResources;

using YourBrand.Portal.Navigation;

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

ModuleLoader.AddServices(builder.Services);

var app = builder.Build();

var moduleBuilder = app.Services.GetRequiredService<ModuleLoader>();
moduleBuilder.ConfigureServices();

await app.Services.Localize();

await app.RunAsync();