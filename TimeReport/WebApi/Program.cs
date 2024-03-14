using System.Globalization;

using Azure.Identity;
using Azure.Storage.Blobs;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Tokens;

using NSwag;
using NSwag.Generation.Processors.Security;

using YourBrand.ApiKeys;
using YourBrand.TimeReport;
using YourBrand.TimeReport.Application;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Hubs;
using YourBrand.TimeReport.Infrastructure;
using YourBrand.TimeReport.Infrastructure.Persistence;
using YourBrand.TimeReport.Services;
using YourBrand.Tenancy;
using YourBrand.Identity;

using Serilog;

using YourBrand;
using YourBrand.Extensions;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "TimeReport"
;
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

var services = builder.Services;

var configuration = builder.Configuration;

services
    .AddApplication(configuration)
    .AddInfrastructure(configuration)
    .AddClients(configuration);

services
    .AddControllers()
    .AddNewtonsoftJson();

services.AddHttpContextAccessor();

services.AddIdentityServices();
services.AddTenantService();

services.AddScoped<IBlobService, BlobService>();

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("sv-SE");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentCulture;

services.AddEndpointsApiExplorer();

services.AddAuthorization();

services.AddAuthenticationServices(builder.Configuration);

services.AddApiKeyAuthentication("https://localhost:5174/api/apikeys/");

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

services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumers(typeof(Program).Assembly);
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

services.AddSignalR();

services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(Program)));

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapObservability();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseOpenApiAndSwaggerUi();
    app.UseSwaggerUi(c => c.DocumentTitle = "Web API v1");
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.MapApplicationRequests();

app.MapGet("/info", () =>
{
    return System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
})
.WithDisplayName("GetInfo")
.WithName("GetInfo")
.WithTags("Info")
.Produces<string>();

app.MapControllers();
app.MapHub<ItemsHub>("/hubs/items");

if (args.Contains("--seed"))
{
    await app.Services.SeedAsync();

    return;
}

app.Run();