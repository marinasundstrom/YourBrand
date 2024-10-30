﻿using Azure.Identity;
using Azure.Storage.Blobs;

using FluentValidation;

using HealthChecks.UI.Client;

using MassTransit;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

using Serilog;

using YourBrand;
using YourBrand.Catalog;
using YourBrand.Catalog.Features;
using YourBrand.Catalog.Features.ProductManagement.Products;
using YourBrand.Catalog.Features.ProductManagement.Products.Variants;
using YourBrand.Catalog.Persistence;
using YourBrand.Catalog.Persistence.Interceptors;
using YourBrand.Catalog.Services;
using YourBrand.Extensions;
using YourBrand.Identity;
using YourBrand.Integration;
using YourBrand.Tenancy;

string ServiceName = "Catalog";

var builder = WebApplication.CreateBuilder(args);

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

builder.AddServiceDefaults();


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

builder.AddDefaultOpenApi();

builder.Services.AddProductsServices();

//builder.Services.AddObservability("Catalog.API", "1.0", builder.Configuration);

builder.Services.AddScoped<AuditableEntitySaveChangesInterceptor>();

builder.Services.AddDbContext<CatalogContext>((sp, options) =>
{
    var connectionString = builder.Configuration.GetValue<string>("yourbrand:catalog-svc:db:connectionstring");

    options.UseSqlServer(connectionString!); //, o => o.EnableRetryOnFailure());

    options.AddInterceptors(
        //sp.GetRequiredService<OutboxSaveChangesInterceptor>(),
        sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

#if DEBUG
    options
        //.LogTo(Console.WriteLine)
        .EnableSensitiveDataLogging();
#endif
});

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
    .AddHealthChecksServices()
    .AddDbContextCheck<CatalogContext>();

builder.Services.AddScoped<ProductVariantsService>();

builder.Services
    .AddUserContext()
    .AddTenantContext();

builder.Services.AddScoped<IDateTime, DateTimeService>();

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddAuthorization();

var reverseProxy = builder.Services
.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapDefaultEndpoints();

//app.MapObservability();

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
    var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    if (args.Contains("--seed"))
    {
        var userContext = scope.ServiceProvider.GetRequiredService<ISettableUserContext>();
        //userContext.SetCurrentUser(TenantConstants.UserAliceId);

        var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
        tenantContext.SetTenantId(TenantConstants.TenantId);

        var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        try
        {
            await Seed2.SeedData(scope.ServiceProvider);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the " +
                "database. Error: {Message}", ex.Message);
        }
        return;
    }
}

app.Run();

static async Task SeedData(CatalogContext context, IConfiguration configuration, ILogger<Program> logger)
{

}