using System.Globalization;

using Azure.Identity;
using Azure.Storage.Blobs;

using MediatR;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

using Skynet.TimeReport;
using Skynet.TimeReport.Application;
using Skynet.TimeReport.Application.Common.Interfaces;
using Skynet.TimeReport.Hubs;
using Skynet.TimeReport.Infrastructure;
using Skynet.TimeReport.Infrastructure.Persistence;
using Skynet.TimeReport.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var Configuration = builder.Configuration;

services
    .AddApplication(Configuration)
    .AddInfrastructure(Configuration);

services
    .AddControllers()
    .AddNewtonsoftJson();

services.AddHttpContextAccessor();

services.AddScoped<ICurrentUserService, CurrentUserService>();
services.AddScoped<IBlobService, BlobService>();

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("sv-SE");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentCulture;

services.AddEndpointsApiExplorer();

// Register the Swagger services
services.AddOpenApiDocument(config =>
{
    config.Title = "Web API";
    config.Version = "v1";
});

services.AddAzureClients(builder =>
{
    // Add a KeyVault client
    //builder.AddSecretClient(keyVaultUrl);

    // Add a Storage account client
    builder.AddBlobServiceClient(Configuration.GetConnectionString("Azure:Storage"))
            .WithVersion(BlobClientOptions.ServiceVersion.V2019_07_07);

    // Use DefaultAzureCredential by default
    builder.UseCredential(new DefaultAzureCredential());
});

services.AddSignalR();

services.AddMediatR(typeof(Program));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseOpenApi();
    app.UseSwaggerUi3(c => c.DocumentTitle = "Web API v1");
}

app.UseHttpsRedirection();

app.UseRouting();

//app.MapApplicationRequests();

app.MapGet("/info", () =>
{
    return System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
})
.WithDisplayName("GetInfo")
.WithName("GetInfo")
.WithTags("Info")
.Produces<string>();

//await app.Services.SeedAsync();

app.MapControllers();
app.MapHub<ItemsHub>("/hubs/items");

app.Run();