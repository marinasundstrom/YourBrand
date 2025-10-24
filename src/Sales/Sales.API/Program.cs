using System.Reflection;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using Serilog;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Notifications.Client;
using YourBrand.Sales;
using YourBrand.Sales.Features;
using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.SubscriptionManagement;
using YourBrand.Sales.Infrastructure;
using YourBrand.Sales.Persistence;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = builder.Configuration["ServiceName"]!;
string ServiceVersion = "1.0";

// Add services to container

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

builder.AddDefaultOpenApi();

builder.AddServiceDefaults();


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

/*

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}
*/

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

builder.Services.AddControllers();

builder.Services.AddSignalR();

builder.Services.AddScoped<AuthForwardHandler>();

builder.Services
    .AddPresentation()
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddPersistence(builder.Configuration);

builder.Services.AddClients(builder.Configuration);

builder.Services
    .AddUserContext()
    .AddTenantContext();

builder.Services.AddScoped<OrderNumberFetcher>();
builder.Services.AddScoped<SubscriptionNumberFetcher>();

//builder.Services.AddTransient<AuthenticationDelegatingHandler>();

builder.Services.AddNotificationsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/notifications/");
},
b =>
{
    //b.AddServiceDiscovery();

    //b.AddHttpMessageHandler<AuthenticationDelegatingHandler>();
});

builder.Services.AddAuthorization();

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services
    .AddHealthChecksServices()
    .AddDbContextCheck<SalesContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseSerilogRequestLogging();

//app.MapObservability();

app.MapDefaultEndpoints();

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

app.MapControllers();

app.MapFeaturesEndpoints();

app.MapHubsForApp();

app.MapHealthChecks();

try
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        if (args.Contains("--seed"))
        {
            if (!SeedArguments.TryGetTenantId(args, out var tenantId))
            {
                Console.Error.WriteLine("Unable to determine tenant id when running with --seed. Usage: dotnet run -- --seed [--tenantId <tenantId>]");
                return;
            }

            var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
            tenantContext.SetTenantId(tenantId);

            Console.WriteLine(tenantContext.TenantId);

            var context = scope.ServiceProvider.GetRequiredService<SalesContext>();

            await context.Database.EnsureDeletedAsync();

            await context.Database.EnsureCreatedAsync();

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