using System.Globalization;
using System.Text.Json.Serialization;

using Azure.Identity;
using Azure.Storage.Blobs;

using MassTransit;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

using Serilog;

using YourBrand.Accounting;
using YourBrand.Accounting.Application;
using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Infrastructure;
using YourBrand.Accounting.Infrastructure.Persistence;
using YourBrand.Accounting.Services;
using YourBrand.Documents.Client;
using YourBrand.Extensions;

using YourBrand.Identity;
using YourBrand.Integration;
using YourBrand.Tenancy;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Accounting";
string ServiceVersion = "1.0";

// Add services to container

builder.Host.UseSerilog((ctx, cfg) => { 
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

/*
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}
*/

builder.Services
    .AddOpenApi(ServiceName, ApiVersions.All, settings => settings.AddJwtSecurity())
    .AddApiVersioningServices();

//builder.Services.AddObservability(ServiceName, ServiceVersion, builder.Configuration);

builder.Services.AddProblemDetails();

var configuration = builder.Configuration;

builder.Services
    .AddApplication(configuration)
    .AddInfrastructure(configuration);

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddUserContext()
    .AddTenantContext();

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddAuthorization();


builder.Services.AddScoped<IBlobService, BlobService>();

// Add services to the container.

builder.Services.AddControllersWithViews();

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    // options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddRazorPages();

builder.Services.AddAzureClients(builder =>
{
    // Add a KeyVault client
    //builder.AddSecretClient(keyVaultUrl);

    // Add a Storage account client
    builder.AddBlobServiceClient(configuration.GetConnectionString("Azure:Storage"))
            .WithVersion(BlobClientOptions.ServiceVersion.V2019_07_07);

    // Use DefaultAzureCredential by default
    builder.UseCredential(new DefaultAzureCredential());
});

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    //x.AddConsumers(typeof(Program).Assembly);

    //x.AddConsumer<TransactionClearedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        //cfg.UseMessageData(messageDataRepository);

        cfg.UseTenancyFilters(context);
        cfg.UseIdentityFilters(context);

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddDocumentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/documents/");
});

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("sv-SE");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentCulture;

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseSerilogRequestLogging();

//app.MapObservability();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApiAndSwaggerUi();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

if (args.Contains("--seed"))
{
    Console.WriteLine("Seeding");

    await app.Services.SeedAsync();
    return;
}

app.Run();