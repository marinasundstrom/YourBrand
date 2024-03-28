using Azure.Identity;

using YourBrand.Carts;

using YourBrand.Catalog;
using YourBrand.Inventory.Client;

using HealthChecks.UI.Client;

using MassTransit;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

using Steeltoe.Common.Http.Discovery;
using Steeltoe.Discovery.Client;

using YourBrand.StoreFront.API;
using YourBrand.StoreFront.API.Features.Cart;
using YourBrand.StoreFront.API.Features.Products;
using YourBrand.StoreFront.API.Persistence;
using YourBrand.StoreFront.API.Features.Brands;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Sales;
using YourBrand.StoreFront.API.Features.Checkout;

using Serilog;
using YourBrand.Analytics.Client;
using System.Text.Json.Serialization;
using System.Text.Json;

string ServiceName = "StoreFront.API";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration)
                        .Enrich.WithProperty("Application", ServiceName)
                        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}

builder.Services.AddCors();

builder.Services.AddOutputCache(options =>
{
    options.AddGetProductsPolicy();

    options.AddGetProductByIdPolicy();
});

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

// Add services to the container.

builder.Services
    .AddOpenApi(ServiceName, ApiVersions.All)
    .AddApiVersioningServices();

builder.Services.AddObservability("StoreFront.API", "1.0", builder.Configuration);

builder.Services.AddSqlServer<StoreFrontContext>(
    builder.Configuration.GetValue<string>("yourbrand:carts-svc:db:connectionstring"),
    c => c.EnableRetryOnFailure());

builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddCartServices();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumers(typeof(Program).Assembly);

    if (builder.Environment.IsProduction())
    {
        x.UsingAzureServiceBus((context, cfg) =>
        {
            cfg.Host($"sb://{builder.Configuration["Azure:ServiceBus:Namespace"]}.servicebus.windows.net");

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

            cfg.ConfigureEndpoints(context);
        });
    }
});

builder.Services
    .AddHealthChecksServices()
    .AddDbContextCheck<StoreFrontContext>();

AddClients(builder);

if (builder.Environment.IsProduction())
{
    builder.Services.AddSingleton<ITokenProvider, AzureADClientCredentialsTokenProvider>();
}
else
{
    builder.Services.AddSingleton<ITokenProvider, IdentityServerClientCredentialsTokenProvider>();
}

builder.Services.AddTransient<AuthenticationDelegatingHandler>();

builder.Services.AddControllers()
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

var app = builder.Build();

app.MapObservability();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApiAndSwaggerUi();
}

app.UseOutputCache();

app.UseHttpsRedirection();

app.UseCors();

app.MapCartEndpoints()
    .MapProductsEndpoints()
    .MapCheckoutEndpoints()
    .MapBrandsEndpoints();

app.MapControllers();

app.MapHealthChecks("/healthz", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

try
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var context = scope.ServiceProvider.GetRequiredService<StoreFrontContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        //await context.Database.EnsureDeletedAsync();
        //await context.Database.EnsureCreatedAsync(); 

        if (args.Contains("--seed"))
        {
            await SeedData(context, configuration, logger);
            return;
        }
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}

await app.RunAsync();

static async Task SeedData(StoreFrontContext context, IConfiguration configuration, ILogger<Program> logger)
{
    try
    {
        await Seed.SeedData(context, configuration);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred seeding the " +
            "database. Error: {Message}", ex.Message);
    }
}

static void AddClients(WebApplicationBuilder builder)
{
    var catalogApiHttpClient = builder.Services.AddCatalogClients((sp, httpClient) =>
    {
        httpClient.BaseAddress = new Uri(builder.Configuration["yourbrand:catalog-svc:url"]!);
    },
    clientBuilder =>
    {
        clientBuilder.AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        clientBuilder.AddStandardResilienceHandler();

        if (builder.Environment.IsDevelopment())
        {
            clientBuilder.AddServiceDiscovery();
        }
    });

    var cartsApiHttpClient = builder.Services.AddCartsClient(new Uri(builder.Configuration["yourbrand:carts-svc:url"]!),
    clientBuilder =>
    {
        clientBuilder.AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        clientBuilder.AddStandardResilienceHandler();

        if (builder.Environment.IsDevelopment())
        {
            clientBuilder.AddServiceDiscovery();
        }
    });

    var salesApiHttpClient = builder.Services.AddSalesClients((sp, http) => {
        http.BaseAddress = new Uri(builder.Configuration["yourbrand:sales-svc:url"]!);
    },
    clientBuilder =>
    {
        clientBuilder.AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        clientBuilder.AddStandardResilienceHandler();

        if (builder.Environment.IsDevelopment())
        {
            clientBuilder.AddServiceDiscovery();
        }
    });

    var inventoryApiHttpClient = builder.Services.AddInventoryClients((sp, http) =>
    {
        http.BaseAddress = new Uri(builder.Configuration["yourbrand:inventory-svc:url"]!);
    },
    clientBuilder =>
    {
        clientBuilder.AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        clientBuilder.AddStandardResilienceHandler();

        if (builder.Environment.IsDevelopment())
        {
            clientBuilder.AddServiceDiscovery();
        }
    });

    var analyticsApiHttpClient = builder.Services.AddAnalyticsClients((sp, http) => {
        http.BaseAddress = new Uri(builder.Configuration["yourbrand:analytics-svc:url"]!);
    },
    clientBuilder =>
    {
        clientBuilder.AddHttpMessageHandler<AuthenticationDelegatingHandler>();

        clientBuilder.AddStandardResilienceHandler();

        if (builder.Environment.IsDevelopment())
        {
            clientBuilder.AddServiceDiscovery();
        }
    });
}

// INFO: Makes Program class visible to IntegrationTests.
public partial class Program { }