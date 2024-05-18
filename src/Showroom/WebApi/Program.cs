﻿using Azure.Identity;
using Azure.Storage.Blobs;

using MassTransit;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Logging;

using Serilog;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.ApiKeys;
using YourBrand.Extensions;
using YourBrand.Identity;
using YourBrand.Integration;
using YourBrand.Showroom;
using YourBrand.Showroom.Application;
using YourBrand.Showroom.Infrastructure;
using YourBrand.Showroom.Infrastructure.Persistence;
using YourBrand.Showroom.WebApi;
using YourBrand.Tenancy;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Showroom";
string ServiceVersion = "1.0";

// Add services to container

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration)
                        .Enrich.WithProperty("Application", ServiceName)
                        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName));

builder.AddServiceDefaults();

/*
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}
*/

builder.Services
    .AddOpenApi(ServiceName, ApiVersions.All)
    .AddApiVersioningServices();

//builder.Services.AddObservability(ServiceName, ServiceVersion, builder.Configuration);

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
    .AddControllers()
    .AddNewtonsoftJson();

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

services.AddAuthenticationServices(builder.Configuration);

//services.AddApiKeyAuthentication("https://localhost:5174/api/apikeys/");

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseSerilogRequestLogging();

//app.MapObservability();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseOpenApiAndSwaggerUi();
}

app.UseHttpsRedirection();

app.UseRouting();

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

if (args.Contains("--seed"))
{
    await app.Services.SeedAsync();

    return;
}

app.Run();