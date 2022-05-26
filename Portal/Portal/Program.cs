using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using YourBrand.Accounting;
using YourBrand.Invoices;
using YourBrand.Portal;
using YourBrand.Showroom;
using YourBrand.TimeReport;
using YourBrand.Transactions;

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

builder.Services
    .AddServices()
    .AddTimeReport()
    .AddShowroom()
    .AddAccounting()
    .AddInvoicing()
    .AddTransactions();

var app = builder.Build();

await app.Services.Localize();

await app.RunAsync();