using Duende.IdentityServer;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Serilog;

using NSwag;
using NSwag.Generation.Processors.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MassTransit;
using YourBrand.IdentityService.Application;
using YourBrand.IdentityService.Domain.Entities;
using AspNetCore.Authentication.ApiKey;
using System.Security.Claims;
using IdentityModel;
using YourBrand.ApiKeys;
using YourBrand.IdentityService.Infrastructure;
using YourBrand.IdentityService.Infrastructure.Persistence;
using Duende.IdentityServer.Services;

namespace YourBrand.IdentityService;

internal static class HostingExtensions
{
    static readonly string MyAllowSpecificOrigins = "MyPolicy";

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddApplication(builder.Configuration)
            .AddInfrastructure(builder.Configuration)
            .AddServices();

        builder.Services.AddOpenApiDocument(document =>
        {
            document.Title = "Identity Service API";
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

        builder.Services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            x.AddConsumers(typeof(Program).Assembly);
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
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

        builder.Services.AddRazorPages();

        builder.Services
            .AddControllers()
            .AddNewtonsoftJson();

        builder.Services.AddIdentity<Person, Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddAspNetIdentity<Person>();

        builder.Services.AddTransient<IProfileService, MyProfileService>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "https://identity.local";
                options.Audience = "myapi";
            })
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                // register your IdentityServer with Google at https://console.developers.google.com
                // enable the Google+ API
                // set the redirect URI to https://localhost:5001/signin-google
                options.ClientId = "copy client ID from Google here";
                options.ClientSecret = "copy client secret from Google here";
            });

        builder.Services.AddApiKeyAuthentication("https://localhost/api/apikeys/");

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            app.UseOpenApi();
            app.UseSwaggerUi3(c =>
            {
                c.DocumentTitle = "Web API v1";
            });
        }

        app.UseStaticFiles();
        app.UseRouting();

        app.UseCors(MyAllowSpecificOrigins);

        app.UseIdentityServer();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        app.MapControllers();

        return app;
    }
}

public class ApiKeyProvider : IApiKeyProvider
{
    private readonly ILogger<IApiKeyProvider> _logger;

    public ApiKeyProvider(ILogger<IApiKeyProvider> logger)
    {
        _logger = logger;
    }

    public async Task<IApiKey> ProvideAsync(string key)
    {
        try
        {
            if(key != "foobar") 
            {
                throw new UnauthorizedAccessException();
            }

            return new ApiKey(key, "api", new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, "api"),
                new Claim(ClaimTypes.NameIdentifier, "api")
            });
        }
        catch (System.Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            throw;
        }
    }
}

class ApiKey : IApiKey
{
    public ApiKey(string key, string owner, List<Claim> claims = null)
    {
        Key = key;
        OwnerName = owner;
        Claims = claims ?? new List<Claim>();
    }

    public string Key { get; }
    public string OwnerName { get; }
    public IReadOnlyCollection<Claim> Claims { get; }
}