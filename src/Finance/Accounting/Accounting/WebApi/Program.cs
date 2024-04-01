using System.Globalization;
using System.Text.Json.Serialization;

using Azure.Identity;
using Azure.Storage.Blobs;

using MassTransit;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

using Serilog;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.Accounting;
using YourBrand.Accounting.Application;
using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Infrastructure;
using YourBrand.Accounting.Infrastructure.Persistence;
using YourBrand.Accounting.Services;
using YourBrand.Documents.Client;
using YourBrand.Extensions;
using YourBrand.Identity;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Accounting";
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
    .AddApplication(configuration)
    .AddInfrastructure(configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentityServices();
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

app.UseHttpsRedirection();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();

if (args.Contains("--seed"))
{
    Console.WriteLine("Seeding");

    await app.Services.SeedAsync();
    return;
}

app.Run();