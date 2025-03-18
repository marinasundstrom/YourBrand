using BlazorApp;
using BlazorApp.Brands;
using BlazorApp.Cart;
using BlazorApp.ProductCategories;
using BlazorApp.Products;

using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazored.Toast;

using Client.Analytics;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using StoreWeb;

using YourBrand.StoreFront;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services
    .AddTransient<CookieHandler>();

builder.Services
    .AddScoped<AnalyticsService>();

builder.Services.AddGeolocationServices();

builder.Services.AddLocalization();

builder.Services
    .AddBlazoredLocalStorage()
    .AddBlazoredSessionStorage();

builder.Services
    .AddHttpClient("WebAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CookieHandler>()
    .AddStandardResilienceHandler();
    
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("WebAPI"));

builder.Services.AddHttpClient<IAuthenticationClient>("WebAPI")
    .AddTypedClient<IAuthenticationClient>((http, sp) => new AuthenticationClient(http));

builder.Services.AddHttpClient<StoreWeb.IOrdersClient>("WebAPI")
    .AddTypedClient<StoreWeb.IOrdersClient>((http, sp) => new StoreWeb.OrdersClient(http));

builder.Services.AddSingleton<RenderingContext, ClientRenderingContext>();

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