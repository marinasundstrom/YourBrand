using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Azure.Identity;
using Azure.Storage.Blobs;

using YourBrand.Showroom.Application;
using YourBrand.Showroom.Infrastructure;
using YourBrand.Showroom.Infrastructure.Persistence;
using YourBrand.Showroom.WebApi;

using MassTransit;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

using NSwag;
using NSwag.Generation.Processors.Security;
using YourBrand.ApiKeys;

using Serilog;

using YourBrand;
using YourBrand.Extensions;

using YourBrand.Showroom;


static class Program
{
    /// <param name="seed">Seed the database</param>
    /// <param name="connectionString">The connection string of the database to act on</param>
    /// <param name="args">The rest of the arguments</param>
    /// <returns></returns>
    static async Task Main(bool seed, string connectionString, string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string ServiceName = "Showroom"
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

        var Configuration = builder.Configuration;

        if (connectionString is not null)
        {
            builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;
        }

        services.AddApplication(Configuration);
        services.AddInfrastructure(Configuration);
        services.AddServices();

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
            builder.AddBlobServiceClient(Configuration.GetConnectionString("Azure:Storage"))
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
                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddStackExchangeRedisCache(o =>
        {
            o.Configuration = Configuration.GetConnectionString("redis");
        });

#if DEBUG
        IdentityModelEventSource.ShowPII = true;
#endif

        services.AddAuthorization();

        services.AddAuthenticationServices(builder.Configuration);

        services.AddApiKeyAuthentication("https://localhost:5174/api/apikeys/");

        var app = builder.Build();

        app.UseSerilogRequestLogging();

        app.MapObservability();

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

        if (seed)
        {
            await app.Services.SeedAsync();

            return;
        }

        app.Run();
    }
}