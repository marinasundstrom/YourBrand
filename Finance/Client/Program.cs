using System.Globalization;

using Accounting.Client;

using Finance.Client;

using Invoices.Client;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor;
using MudBlazor.Services;

using Transactions.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

CultureInfo? culture = new("sv-SE");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services.AddInvoicesClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}invoices/");
});

builder.Services.AddTransactionsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}transactions/");
});

builder.Services.AddAccountingClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}accounting/");
});

await builder.Build().RunAsync();