using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Commerce.Client;
using YourBrand.Commerce.Client;
using System.Globalization;
using Blazor.Analytics;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient(nameof(ProductsClient), (sp, http) => {
                http.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            })
            .AddTypedClient<IProductsClient>((http, sp) => new ProductsClient(http));

CultureInfo? culture = new("sv-SE");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

builder.Services.AddGoogleAnalytics("YOUR_GTAG_ID");

await builder.Build().RunAsync();
