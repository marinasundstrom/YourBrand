using System.Text.Json.Serialization;

using MassTransit;

using MediatR;

using YourBrand.Catalog.Application;
using YourBrand.Catalog.Application.Common.Interfaces;
using YourBrand.Catalog.Infrastructure;
using YourBrand.Catalog.Infrastructure.Persistence;
using YourBrand.Documents.Client;
using YourBrand.Payments.Client;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs;
using Azure.Identity;
using YourBrand.Identity;


using Serilog;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Catalog;
var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Catalog"
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


var Configuration = builder.Configuration;

if (args.Contains("--connection-string"))
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] = args[args.ToList().IndexOf("--connection-string") + 1];
}

builder.Services
    .AddApplication()
    .AddInfrastructure(Configuration);

builder.Services.AddControllers();

// Set the JSON serializer options
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    // options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentityServices();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerDocument(c =>
{
    c.Title = "Catalog API";
    c.Version = "0.1";
});

builder.Services.AddAzureClients(builder =>
        {
            // Add a KeyVault client
            //builder.AddSecretClient(keyVaultUrl);

            // Add a Storage account client
            builder.AddBlobServiceClient(Configuration.GetConnectionString("Azure:Storage"))
                            .WithVersion(BlobClientOptions.ServiceVersion.V2019_07_07);

            // Use DefaultAzureCredential by default
            builder.UseCredential(new DefaultAzureCredential());
        });

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

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapObservability();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
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