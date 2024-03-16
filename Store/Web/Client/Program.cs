using BlazorApp;

using BlazorApp.Cart;
using BlazorApp.ProductCategories;
using BlazorApp.Products;
using BlazorApp.Brands;

using Blazored.Toast;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

using YourBrand.StoreFront;
using Client.Analytics;
using Blazored.LocalStorage;
using Blazored.SessionStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services
    .AddTransient<CookieHandler>();

builder.Services
    .AddScoped<AnalyticsService>();

builder.Services.AddGeolocationServices();

builder.Services
    .AddBlazoredLocalStorage()
    .AddBlazoredSessionStorage();

builder.Services
    .AddHttpClient("WebAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CookieHandler>()
    .AddStandardResilienceHandler();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("WebAPI"));

builder.Services.AddScoped<RenderingContext, ClientRenderingContext>();

var baseUri = new Uri(builder.HostEnvironment.BaseAddress + "storefront/");

var catalogApiHttpClient = builder.Services.AddStoreFrontClients(baseUri,
clientBuilder =>
{
#if !DEBUG
    clientBuilder.AddStandardResilienceHandler();
#endif
});

builder.Services
    .AddProductsServices()
    .AddProductCategoriesServices()
    .AddCartServices()
    .AddBrandsServices();

builder.Services.AddBlazoredToast();

var app = builder.Build();

var analyticsService = app.Services.GetRequiredService<Client.Analytics.AnalyticsService>();
await analyticsService.Init();

await app.RunAsync();