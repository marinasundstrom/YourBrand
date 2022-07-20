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

static class Program
{
    /// <param name="seed">Seed the database</param>
    /// <param name="args">The rest of the arguments</param>
    /// <returns></returns>
    static async Task Main(bool seed, string? connectionString, string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        var Configuration = builder.Configuration;

        builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

        services
            .AddApplication(Configuration)
            .AddInfrastructure(Configuration)
            .AddClients(Configuration);

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

        // Register the Swagger services
        services.AddOpenApiDocument(config =>
        {
            config.Title = "TimeReport API";
            config.Version = "v1";


            config.AddSecurity("JWT", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });

            config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = "https://identity.local";
                        options.Audience = "myapi";

                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            NameClaimType = "name"
                        };

                        //options.TokenValidationParameters.ValidateAudience = false;

                        //options.Audience = "openid";

                        //options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                    });

        services.AddApiKeyAuthentication("https://localhost/api/apikeys/");

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

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumers(typeof(Program).Assembly);
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        })
        .AddMassTransitHostedService(true)
        .AddGenericRequestClient();

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

        if (seed)
        {
            await app.Services.SeedAsync();

            return;
        }

        app.Run();
    }
}