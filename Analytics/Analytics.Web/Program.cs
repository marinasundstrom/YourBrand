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

using Serilog;

using YourBrand;
using YourBrand.Extensions;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Analytics"
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

var configuration = builder.Configuration;

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

builder.Services.AddSignalR();

builder.Services.AddMemoryCache();

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

builder.Services.AddAuthorization();

builder.Services.AddAuthenticationServices(builder.Configuration);

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

app.UseSerilogRequestLogging();

app.MapObservability();

app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseOpenApi();
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
        //await context.Database.EnsureDeletedAsync();
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