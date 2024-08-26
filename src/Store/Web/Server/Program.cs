using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Azure.Identity;

using BlazorApp;
using BlazorApp.Brands;
using BlazorApp.Cart;
using BlazorApp.Data;
using BlazorApp.Extensions;
using BlazorApp.ProductCategories;
using BlazorApp.Products;

using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazored.Toast;

using Client.Analytics;

using HealthChecks.UI.Client;

using MassTransit;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;

using Serilog;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Integration;
using YourBrand.StoreFront;

string MyAllowSpecificOrigins = nameof(MyAllowSpecificOrigins);

string ServiceName = "Store.Web";
string ServiceVersion = "1.0";

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

/*
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}
*/

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(builder.Configuration)
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        var headers = builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]?.Split(',') ?? [];
        foreach (var header in headers)
        {
            var (key, value) = header.Split('=') switch
            {
            [string k, string v] => (k, v),
                var v => throw new Exception($"Invalid header format {v}")
            };

            options.Headers.Add(key, value);
        }
        options.ResourceAttributes.Add("service.name", ServiceName);
    })
    .Enrich.WithProperty("Application", ServiceName)
    .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName);
});

builder.Services.AddRateLimiterForIPAddress(builder.Configuration);

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy(OutputCachePolicyNames.GetProductsExpire20, builder =>
    {
        builder.Expire(TimeSpan.FromSeconds(20));
        builder.SetVaryByQuery("page", "pageSize", "searchTerm");
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(
                    "https://yourbrand-store-web.kindgrass-70ab37e8.swedencentral.azurecontainerapps.io",
                    "https://localhost:7188",
                    "https://localhost:8080")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

AddClients(builder);

builder.Services
    .AddProductsServices()
    .AddProductCategoriesServices()
    .AddCartServices()
    .AddBrandsServices();

builder.Services
    .AddScoped<AnalyticsService>();

builder.Services.AddGeolocationServices();

builder.Services
    .AddBlazoredLocalStorage()
    .AddBlazoredSessionStorage();

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureAppConfiguration(options =>
        options.Connect(
            new Uri($"https://{builder.Configuration["Azure:AppConfig:Name"]}.azconfig.io"),
            new DefaultAzureCredential()));

    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{builder.Configuration["Azure:KeyVault:Name"]}.vault.azure.net/"),
        new DefaultAzureCredential());
}

builder.Services
    .AddOpenApi(ServiceName)
    .AddApiVersioningServices();

//builder.Services.AddObservability(serviceName, serviceVersion, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAuthentication("MicrosoftOidc")
    .AddOpenIdConnect("MicrosoftOidc", oidcOptions =>
{
    oidcOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    builder.Configuration.Bind("Local", oidcOptions);

    oidcOptions.MapInboundClaims = true;
    oidcOptions.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
    oidcOptions.TokenValidationParameters.RoleClaimType = "role";
})
.AddCookie("Cookies");

builder.Services.ConfigureCookieOidcRefresh("Cookies", "MicrosoftOidc");

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(c => c.UseInMemoryDatabase("db"));

builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();

builder.Services.AddSingleton<RenderingContext, ServerRenderingContext>();

builder.Services.AddSingleton<RequestContext>();

builder.Services.AddLocalization();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumers(typeof(Program).Assembly);

    if (builder.Environment.IsProduction())
    {
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host($"sb://{builder.Configuration["Azure:ServiceBus:Namespace"]}.servicebus.windows.net");

            cfg.UseTenancyFilters(context);
            cfg.UseIdentityFilters(context);

            cfg.ConfigureEndpoints(context);
        });
    }
    else
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            var rabbitmqHost = builder.Configuration["RABBITMQ_HOST"] ?? "localhost";

            cfg.Host(rabbitmqHost, "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.UseTenancyFilters(context);
            cfg.UseIdentityFilters(context);

            cfg.ConfigureEndpoints(context);
        });
    }
});

builder.Services
    .AddHealthChecks();

builder.Services.AddBlazoredToast();

var reverseProxy = builder.Services.AddReverseProxy();

if (builder.Environment.IsDevelopment())
{
    reverseProxy.LoadFromMemory();
}
else
{
    reverseProxy.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
}

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapDefaultEndpoints();

app.MapReverseProxy();

app.UseStatusCodePagesWithRedirects("/error/{0}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseOpenApi();

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(MyAllowSpecificOrigins);

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddAdditionalAssemblies(typeof(BlazorApp.CookieHandler).Assembly)
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode();

app.MapGroup("/authentication").MapLoginAndLogout();

app.MapGet("/requires-auth", (ClaimsPrincipal user) => $"Hello, {user.Identity?.Name}!").RequireAuthorization();

app.MapGet("/api/weatherforecast", async (DateOnly startDate, IWeatherForecastService weatherForecastService, CancellationToken cancellationToken) =>
{
    var forecasts = await weatherForecastService.GetWeatherForecasts(startDate, cancellationToken);
    return Results.Ok(forecasts);
})
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapHealthChecks("/healthz", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();

static void AddClients(WebApplicationBuilder builder)
{
    var catalogApiHttpClient = builder.Services.AddStoreFrontClients(static (sp, http) =>
    {
        var hostEnv = sp.GetRequiredService<IHostEnvironment>();

        Uri? baseUrl;

        if (hostEnv.IsDevelopment())
        {
            var server = sp.GetRequiredService<IServer>();
            var serverAddress = server.Features.Get<IServerAddressesFeature>()!.Addresses.First();
            baseUrl = new Uri(serverAddress + "/storefront/");
        }
        else
        {
            var serverAddress = "http://yourbrand-store-web";
            baseUrl = new Uri(serverAddress + "/storefront/");
        }

        http.BaseAddress = baseUrl;
    },
    static (clientBuilder) =>
    {
        //clientBuilder.AddStandardResilienceHandler();
    });
}