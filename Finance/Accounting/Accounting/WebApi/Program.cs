using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

using YourBrand.Accounting.Application;
using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Infrastructure;
using YourBrand.Accounting.Infrastructure.Persistence;
using YourBrand.Accounting.Services;

using Azure.Identity;
using Azure.Storage.Blobs;

using YourBrand.Documents.Client;

using MassTransit;

using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

if(args.Contains("--connection-string")) 
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] = args[args.ToList().IndexOf("--connection-string") + 1];
}

builder.Services
    .AddApplication(configuration)
    .AddInfrastructure(configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
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

builder.Services.AddSwaggerDocument(c =>
{
    c.Title = "Accounting API";
    c.Version = "0.1";
});

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
})
.AddMassTransitHostedService(true)
.AddGenericRequestClient();

builder.Services.AddDocumentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{configuration.GetServiceUri("nginx")}/api/documents/");
});

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("sv-SE");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentCulture;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
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

if(args.Contains("--seed")) 
{
    Console.WriteLine("Seeding");

    await app.Services.SeedAsync();
    return;
}

app.Run();