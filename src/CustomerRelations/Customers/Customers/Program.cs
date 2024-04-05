using System.Text.Json.Serialization;

using Azure.Identity;
using Azure.Storage.Blobs;

using MassTransit;

using Microsoft.Extensions.Azure;

using Serilog;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.Customers;
using YourBrand.Customers.Application;
using YourBrand.Customers.Application.Services;
using YourBrand.Customers.Infrastructure;
using YourBrand.Customers.Infrastructure.Persistence;
using YourBrand.Customers.Infrastructure.Services;
using YourBrand.Documents.Client;
using YourBrand.Extensions;
using YourBrand.Identity;
using YourBrand.Payments.Client;
using YourBrand.Tenancy;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Customers";
string ServiceVersion = "1.0";

// Add services to container

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration)
                        .Enrich.WithProperty("Application", ServiceName)
                        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}

builder.Services
    .AddOpenApi(ServiceName, ApiVersions.All)
    .AddApiVersioningServices();

builder.Services.AddObservability(ServiceName, ServiceVersion, builder.Configuration);

builder.Services.AddProblemDetails();

var configuration = builder.Configuration;

builder.Services
    .AddApplication()
    .AddInfrastructure(configuration);

builder.Services.AddControllers();

// Set the JSON serializer options
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    // options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddIdentityServices()
    .AddTenantContext();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    //x.AddConsumers(typeof(Program).Assembly);

    //x.AddRequestClient<IncomingTransactionBatch>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddDocumentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/documents/");
});

builder.Services.AddPaymentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/payments/");
});

builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

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

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapObservability();

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

app.MapGet("/", () => "Hello World!");

app.MapControllers();

if (args.Contains("--seed"))
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();