using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;

using Serilog;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Identity;
using YourBrand.IdentityManagement;
using YourBrand.IdentityManagement.Application;
using YourBrand.IdentityManagement.Consumers;
using YourBrand.IdentityManagement.Infrastructure;
using YourBrand.Integration;
using YourBrand.Tenancy;

string MyAllowSpecificOrigins = "MyPolicy";


var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Identity Management";
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

var services = builder.Services;

var configuration = builder.Configuration;

builder.Services
           .AddApplication(builder.Configuration)
           .AddInfrastructure(builder.Configuration)
           .AddServices();

builder.Services
    .AddUserContext()
    .AddTenantContext();

services
    .AddControllers();

services.AddHttpContextAccessor();

services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      builder =>
                      {
                          builder
                              .AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                      });
});

services.AddSignalR();

services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddAppConsumers();

    //x.AddConsumer(typeof(PersonCreatedConsumer), typeof(PersonCreatedConsumerDefinition));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseTenancyFilters(context);
        cfg.UseIdentityFilters(context);

        cfg.ConfigureEndpoints(context);
    });
});

services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

#if DEBUG
IdentityModelEventSource.ShowPII = true;
#endif

var app = builder.ConfigureServices();

if (args.Contains("--seed"))
{
    if (!SeedArguments.TryGetTenantId(args, out var tenantId))
    {
        Console.Error.WriteLine("Tenant id is required when running with --seed. Usage: dotnet run -- --seed -- <tenantId>");
        return;
    }

    await app.EnsureSeedData(tenantId);

    //await app.Services.SeedAsync();

    return;
}

app.ConfigurePipeline();

app.Run();