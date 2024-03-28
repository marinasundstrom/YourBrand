using Azure.Identity;

using HealthChecks.UI.Client;

using MassTransit;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.API.Features;
using YourBrand.Sales.API.Features.OrderManagement;
using YourBrand.Sales.API.Features.OrderManagement.Orders;
using YourBrand.Sales.API.Infrastructure;
using YourBrand.Sales.API.Persistence;
using YourBrand.Notifications.Client;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Sales.API;
using Serilog;
using System.Reflection;
using YourBrand.Sales.Features.Common;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = builder.Configuration["ServiceName"]!;
string ServiceVersion = "1.0";

// Add services to container

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration)
                        .Enrich.WithProperty("Application", ServiceName)
                        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName));

builder.Services
    .AddOpenApi(ServiceName, ApiVersions.All)
    .AddApiVersioningServices();

builder.Services.AddObservability(ServiceName, ServiceVersion, builder.Configuration);

builder.Services.AddProblemDetails();

builder.Services.AddHttpContextAccessor();

string GetCartsExpire20 = nameof(GetCartsExpire20);

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy(GetCartsExpire20, builder =>
    {
        builder.Expire(TimeSpan.FromSeconds(20));
        builder.SetVaryByQuery("page", "pageSize", "searchTerm");
    });
});

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureAppConfiguration(builder.Configuration);

    builder.Configuration.AddAzureKeyVault(builder.Configuration);
}

builder.Services.AddServiceBus(bus =>
{
    bus.AddConsumers(Assembly.GetExecutingAssembly());

    if (builder.Environment.IsDevelopment())
    {
        bus.UsingRabbitMQ(builder.Configuration);
    }
    else if (builder.Environment.IsProduction())
    {
        bus.UsingAzureServiceBus(builder.Configuration);
    }
});

builder.Services.AddSignalR();

builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<ITenantService, TenantService>();

builder.Services.AddNotificationsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/notifications/");
}, b => { });

//builder.Services.AddTenantService();
//builder.Services.AddCurrentUserService();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddAuthentication_IdentityServer(builder.Configuration);
}
else if (builder.Environment.IsProduction())
{
    builder.Services.AddAuthentication_Entra(builder.Configuration);
}

builder.Services.AddAuthorization();

builder.Services
    .AddHealthChecksServices()
    .AddDbContextCheck<SalesContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSerilogRequestLogging();

app.MapObservability();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApiAndSwaggerUi();
}

app.UseOutputCache();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapFeaturesEndpoints();

app.MapHubsForApp();

app.MapHealthChecks();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var context = scope.ServiceProvider.GetRequiredService<SalesContext>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        if (args.Contains("--seed"))
        {
            await context.Database.EnsureDeletedAsync();

            await context.Database.EnsureCreatedAsync();

            await SeedData(context, configuration, logger);
            return;
        }
        else 
        {
            await context.Database.MigrateAsync();
        }
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}

await app.RunAsync();

static async Task SeedData(SalesContext context, IConfiguration configuration, ILogger<Program> logger)
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

// INFO: Makes Program class visible to IntegrationTests.
public partial class Program { }