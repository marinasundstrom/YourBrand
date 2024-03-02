using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Azure.Identity;
using Azure.Storage.Blobs;

using YourBrand.Application;
using YourBrand.Infrastructure;
using YourBrand.Infrastructure.Persistence;
using YourBrand.WebApi;
using YourBrand.WebApi.Hubs;

using MassTransit;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

using NSwag;
using NSwag.Generation.Processors.Security;
using YourBrand.HumanResources.Contracts;

using YourBrand.Consumers;

static class Program
{
    static readonly string MyAllowSpecificOrigins = "MyPolicy";

    /// <param name="seed">Seed the database</param>
    /// <param name="connectionString">The connection string of the database to act on</param>
    /// <param name="args">The rest of the arguments</param>
    /// <returns></returns>
    static async Task Main(bool seed, string connectionString, string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        var Configuration = builder.Configuration;

        if (connectionString is not null)
        {
            builder.Configuration["ConnectionStrings:DefaultConnection"] = connectionString;

            Console.WriteLine(builder.Configuration["ConnectionStrings:DefaultConnection"]);
        }

        services.AddApplication(Configuration);
        services.AddInfrastructure(Configuration);
        services.AddServices();

        services
            .AddControllers()
            .AddNewtonsoftJson();

        services.AddHttpContextAccessor();

        services.AddEndpointsApiExplorer();

        // Register the Swagger services
        services.AddOpenApiDocument(document =>
        {
            document.Title = "Web API";
            document.Version = "v1";

            document.AddSecurity("JWT", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });

            document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
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

        services.AddCors(options =>
        {
            options.AddPolicy(name: MyAllowSpecificOrigins,
                            builder =>
                            {
                                builder
                                    .AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                            });
        });

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

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = "https://localhost:5040";
                        options.Audience = "myapi";

                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            NameClaimType = "name"

                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnTokenValidated = context =>
                            {
                                // Add the access_token as a claim, as we may actually need it
                                var accessToken = context.SecurityToken as JwtSecurityToken;
                                if (accessToken != null)
                                {
                                    ClaimsIdentity? identity = context.Principal.Identity as ClaimsIdentity;
                                    if (identity != null)
                                    {
                                        identity.AddClaim(new Claim("access_token", accessToken.RawData));
                                    }
                                }

                                return Task.CompletedTask;
                            }
                        };

                        //options.TokenValidationParameters.ValidateAudience = false;

                        //options.Audience = "openid";

                        //options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
                    });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseOpenApi();
            app.UseSwaggerUi3(c =>
            {
                c.DocumentTitle = "Web API v1";
            });
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors(MyAllowSpecificOrigins);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapApplicationRequests();

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
        app.MapHub<SomethingHub>("/hubs/something");
        app.MapHub<WorkerHub>("/hubs/worker");
        app.MapHub<NotificationHub>("/hubs/notifications");

        if (seed)
        {
            await app.Services.SeedAsync();

            return;
        }

        app.Run();
    }
}