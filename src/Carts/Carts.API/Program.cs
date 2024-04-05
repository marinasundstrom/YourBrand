using Azure.Identity;

using HealthChecks.UI.Client;

using MassTransit;

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

using Serilog;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.Carts;
using YourBrand.Carts.Features.CartsManagement;
using YourBrand.Carts.Persistence;
using YourBrand.Carts.Services;
using YourBrand.Extensions;

using YourBrand.Carts.Persistence.Interceptors;

using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Domain;

string ServiceName = "Carts.API";

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration)
                        .Enrich.WithProperty("Application", ServiceName)
                        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}

string GetCartsExpire20 = nameof(GetCartsExpire20);

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy(GetCartsExpire20, builder =>
    {
        builder.Expire(TimeSpan.FromSeconds(20));
        builder.SetVaryByQuery("page", "pageSize", "searchTerm");
    });
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

builder.Services.AddObservability("Carts.API", "1.0", builder.Configuration);

builder.Services.AddScoped<AuditableEntitySaveChangesInterceptor>();

builder.Services.AddDbContext<CartsContext>((sp, options) =>
{
    var connectionString = builder.Configuration.GetValue<string>("yourbrand:carts-svc:db:connectionstring");

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

builder.Services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining<Program>());

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

            cfg.UseConsumeFilter(typeof(ReadTenantIdConsumeFilter<>), context);

            cfg.ConfigureEndpoints(context);
        });
    }
});

builder.Services
    .AddHealthChecksServices()
    .AddDbContextCheck<CartsContext>();

builder.Services
    .AddIdentityServices()
    .AddTenantContext();

builder.Services.AddScoped<IDateTime, DateTimeService>();

var app = builder.Build();

app.MapObservability();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApiAndSwaggerUi();
}

app.UseOutputCache();

app.UseHttpsRedirection();

app.MapCartsEndpoints();

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

        if (args.Contains("--seed"))
        {
            var tenantContext = scope.ServiceProvider.GetRequiredService<ITenantContext>();
            tenantContext.SetTenantId(TenantConstants.TenantId);

            var context = scope.ServiceProvider.GetRequiredService<CartsContext>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

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

static async Task SeedData(CartsContext context, IConfiguration configuration, ILogger<Program> logger)
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
