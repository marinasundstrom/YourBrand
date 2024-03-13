using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.RateLimiting;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NSwag;
using NSwag.Generation.Processors.Security;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using YourBrand.Analytics.Application;
using YourBrand.Analytics.Application.Services;
using YourBrand.Analytics.Infrastructure.Persistence;
using YourBrand.Analytics.Web;
using YourBrand.Analytics.Web.Middleware;
using YourBrand.Analytics.Web.Services;
using Microsoft.Extensions.Azure;
using Azure.Storage.Blobs;
using Azure.Identity;
using Microsoft.Extensions.Caching.Memory;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

// Define some important constants to initialize tracing with
var serviceName = "YourBrand.Analytics";
var serviceVersion = "1.0.0";

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:5174", "https://localhost:5001")
                          .AllowAnyHeader().AllowAnyMethod();
                      });
});

// Add services to the container.

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

builder.Services.AddVersionedApiExplorer(option =>
        {
            option.GroupNameFormat = "VVV";
            option.SubstituteApiVersionInUrl = true;
        });

// Register the Swagger services

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
var provider = builder.Services
    .BuildServiceProvider()
    .GetRequiredService<IApiVersionDescriptionProvider>();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

foreach (ApiVersionDescription description in provider.ApiVersionDescriptions)
{
    builder.Services.AddOpenApiDocument(config =>
    {
        config.DocumentName = $"v{description.ApiVersion}";
        config.PostProcess = document =>
        {
            document.Info.Title = "Analytics API";
            document.Info.Version = $"v{description.ApiVersion.ToString()}";
        };
        config.ApiGroupNames = new[] { description.ApiVersion.ToString() };

        config.DefaultReferenceTypeNullHandling = NJsonSchema.Generation.ReferenceTypeNullHandling.NotNull;

        config.AddSecurity("JWT", new OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Type into the textbox: Bearer {your JWT token}."
        });

        config.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
    });
}

builder.Services.AddSignalR();

builder.Services.AddMemoryCache();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = "https://localhost:5031";
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
                                    ClaimsIdentity? identity = context?.Principal?.Identity as ClaimsIdentity;
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

builder.Services.AddAuthorization();

builder.Services.AddUniverse(builder.Configuration);

builder.Services.AddTransient<ExceptionHandlingMiddleware>();

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

builder.Services.AddMassTransitForApp();

// Configure important OpenTelemetry settings, the console exporter, and instrumentation library
builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
        .AddConsoleExporter()
        .AddZipkinExporter(o =>
        {
            o.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
            o.ExportProcessorType = OpenTelemetry.ExportProcessorType.Simple;
        })
        .AddSource(serviceName)
        .AddSource("MassTransit")
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddSqlClientInstrumentation()
        .AddMassTransitInstrumentation();
//        .AddRedisInstrumentation();
});

builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = (context, cancellationToken) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

            // context.Lease.GetAllMetadata().ToList()
            //    .ForEach(m => app.Logger.LogWarning($"Rate limit exceeded: {m.Key} {m.Value}"));

            return new ValueTask();
        };

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddTokenBucketLimiter("MyControllerPolicy", options =>
    {
        options.TokenLimit = 5;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

#if DEBUG
        options.QueueLimit = 1;
#else
        options.QueueLimit = 1000;
#endif

        options.ReplenishmentPeriod = TimeSpan.FromSeconds(5);
        options.TokensPerPeriod = 1;
    });
});

var app = builder.Build();

app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseOpenApi();
    app.UseSwaggerUi3();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHubsForApp();

app.UseRateLimiter();

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var dbProviderName = context.Database.ProviderName;

    if (dbProviderName!.Contains("SqlServer"))
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync(); 

        try
        {
            await ApplyMigrations(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred when applying migrations to the " +
                "database. Error: {Message}", ex.Message);
        }
    }

    if (args.Contains("--seed"))
    {
        try
        {
            await Seed.SeedData(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred seeding the " +
                "database. Error: {Message}", ex.Message);
        }

        return;
    }
}

app.Run();

static async Task ApplyMigrations(ApplicationDbContext context)
{
    var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
    if (pendingMigrations.Count() > 0)
    {
        await context.Database.MigrateAsync();
    }
}

// INFO: Makes Program class visible to IntegrationTests.
public partial class Program { }