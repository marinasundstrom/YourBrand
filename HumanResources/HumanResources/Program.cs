using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Azure.Identity;

using YourBrand.HumanResources.Application;
using YourBrand.HumanResources.Infrastructure;
using YourBrand.HumanResources.Infrastructure.Persistence;

using MassTransit;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

using NSwag;
using NSwag.Generation.Processors.Security;
using YourBrand.HumanResources.Consumers;
using YourBrand.HumanResources;

using Serilog;

using YourBrand;
using YourBrand.Extensions;

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

        string ServiceName = "HumanResource"
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

            Console.WriteLine(builder.Configuration["ConnectionStrings:DefaultConnection"]);
        }

        builder.Services
                   .AddApplication(builder.Configuration)
                   .AddInfrastructure(builder.Configuration)
                   .AddServices();

        services
            .AddControllers()
            .AddNewtonsoftJson();

        services.AddHttpContextAccessor();

        services.AddEndpointsApiExplorer();

        // Register the Swagger services
        builder.Services.AddOpenApiDocument(document =>
        {
            document.Title = "Human Resources API";
            document.Version = "v1";

            document.AddSecurity("JWT", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });

            document.AddSecurity("ApiKey", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "X-API-KEY",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: {your API key}."
            });

            document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
            document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("ApiKey"));
        });

        builder.Services.AddCors(options =>
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

        services.AddSignalR();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddAppConsumers();

            //x.AddConsumer(typeof(PersonCreatedConsumer), typeof(PersonCreatedConsumerDefinition));

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
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

        app.UseSerilogRequestLogging();

        app.MapObservability();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        if (seed)
        {
            await app.Services.SeedAsync();

            return;
        }

        app.Run();
    }
}