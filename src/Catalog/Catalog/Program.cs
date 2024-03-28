using Azure.Identity;
using Azure.Storage.Blobs;

using YourBrand.Catalog.Common;
using YourBrand.Catalog.Features;
using YourBrand.Catalog.Features.ProductManagement.Products;
using YourBrand.Catalog.Features.ProductManagement.Products.Variants;
using YourBrand.Catalog.Persistence;

using FluentValidation;

using HealthChecks.UI.Client;

using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Catalog;

using Serilog;

string ServiceName = "Catalog";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration)
                        .Enrich.WithProperty("Application", ServiceName)
                        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}

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

builder.Services.AddValidatorsFromAssemblyContaining(typeof(Program));

builder.Services.AddAzureClients(clientBuilder =>
{
    if (builder.Environment.IsProduction())
    {
        // Add a KeyVault client
        clientBuilder.AddSecretClient(new Uri($"https://{builder.Configuration["Azure:KeyVault:Name"]}.vault.azure.net/"));
    }

    // Add a Storage account client
    if (builder.Environment.IsDevelopment())
    {
        clientBuilder.AddBlobServiceClient(builder.Configuration["Azure:StorageAccount:ConnectionString"])
                        .WithVersion(BlobClientOptions.ServiceVersion.V2019_07_07);
    }
    else
    {
        clientBuilder.AddBlobServiceClient(new Uri($"https://{builder.Configuration["Azure:StorageAccount:Name"]}.blob.core.windows.net"));
    }

    // Use DefaultAzureCredential by default
    clientBuilder.UseCredential(new DefaultAzureCredential());
});

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

builder.Services
    .AddOpenApi(ServiceName, ApiVersions.All)
    .AddApiVersioningServices();

builder.Services.AddProductsServices();

builder.Services.AddObservability("Catalog.API", "1.0", builder.Configuration);

builder.Services.AddSqlServer<CatalogContext>(
    builder.Configuration.GetValue<string>("yourbrand:catalog-svc:db:connectionstring"));

builder.Services.AddMediatR(c => c.RegisterServicesFromAssemblyContaining<Program>());

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
    .AddDbContextCheck<CatalogContext>();

builder.Services.AddScoped<ProductVariantsService>();

builder.Services.AddScoped<ICurrentUserService>(sp => null!);

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddAuthorization();

var reverseProxy = builder.Services
.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapObservability();

app.MapReverseProxy();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApiAndSwaggerUi();
}

app.UseOutputCache();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapFeaturesEndpoints();

app.MapHealthChecks("/healthz", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    //await context.Database.EnsureDeletedAsync();
    //await context.Database.EnsureCreatedAsync(); 

    if (args.Contains("--seed"))
    {
        await SeedData(context, configuration, logger);
        return;
    }
}

app.Run();

static async Task SeedData(CatalogContext context, IConfiguration configuration, ILogger<Program> logger)
{
    try
    {
        await Seed2.SeedData(context, configuration);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred seeding the " +
            "database. Error: {Message}", ex.Message);
    }
}