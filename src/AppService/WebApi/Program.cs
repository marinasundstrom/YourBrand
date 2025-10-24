using Azure.Identity;
using Azure.Storage.Blobs;

using MassTransit;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Logging;

using Serilog;

using YourBrand;
using YourBrand.Application;
using YourBrand.Extensions;
using YourBrand.Identity;
using YourBrand.Infrastructure;
using YourBrand.Infrastructure.Persistence;
using YourBrand.Integration;
using YourBrand.Tenancy;
using YourBrand.WebApi;
using YourBrand.WebApi.Hubs;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


var builder = WebApplication.CreateBuilder(args);

string ServiceName = "AppService";
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

services.AddApplication(configuration);
services.AddInfrastructure(configuration);
services.AddServices();

builder.Services
    .AddUserContext()
    .AddTenantContext();

services
    .AddControllers();

services.AddHttpContextAccessor();

services.AddEndpointsApiExplorer();

services.AddAzureClients(builder =>
{
    // Add a KeyVault client
    //builder.AddSecretClient(keyVaultUrl);

    // Add a Storage account client
    builder.AddBlobServiceClient(configuration.GetConnectionString("Azure:Storage"))
                    .WithVersion(BlobClientOptions.ServiceVersion.V2019_07_07);

    // Use DefaultAzureCredential by default
    builder.UseCredential(new DefaultAzureCredential());
});

services.AddSignalR();

services.AddCors(options =>
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

services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();

services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumers(typeof(Program).Assembly);
    //x.AddConsumer(typeof(UserCreatedConsumer), typeof(UserCreatedConsumerDefinition));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseTenancyFilters(context);
        cfg.UseIdentityFilters(context);

        cfg.ConfigureEndpoints(context);
    });
});

services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = configuration.GetConnectionString("redis");
});

#if DEBUG
IdentityModelEventSource.ShowPII = true;
#endif

services.AddAuthorization();

services.AddAuthenticationServices(configuration);

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseDefaultOpenApi();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/info", () =>
{
    return System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
})
.WithDisplayName("GetInfo")
.WithName("GetInfo")
.WithTags("Info")
.Produces<string>();

app.MapControllers();
app.MapHub<SomethingHub>("/hubs/something");
app.MapHub<WorkerHub>("/hubs/worker");
app.MapHub<NotificationHub>("/hubs/notifications");

if (args.Contains("--seed"))
{
    if (!SeedArguments.TryGetTenantId(args, out var tenantId))
    {
        Console.Error.WriteLine("Tenant id is required when running with --seed. Usage: dotnet run -- --seed -- <tenantId>");
        return;
    }

    await app.Services.SeedAsync(tenantId);

    return;
}

app.Run();