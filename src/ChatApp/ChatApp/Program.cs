using HealthChecks.UI.Client;
using Microsoft.EntityFrameworkCore;
using NSwag.AspNetCore;
using YourBrand.ChatApp;
using YourBrand.ChatApp.Extensions;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.ChatApp.Web.Extensions;
using YourBrand.ChatApp.Web.Middleware;
using YourBrand.ChatApp.Web;

using YourBrand.Extensions;
using YourBrand.Identity;
using YourBrand.Integration;
using YourBrand.Tenancy;

using Serilog;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "ChatApp";
string ServiceVersion = "1.0";

var configuration = builder.Configuration;

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration)
                        .Enrich.WithProperty("Application", ServiceName)
                        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName));


builder.Services.AddProblemDetails();

builder.Services.AddAuthorization();

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddControllers();

builder.Services
    .AddUniverse(configuration)
    .AddRateLimiter()
    .AddCaching(configuration)
    //.AddFeatureManagement()
    .AddSignalR();

/*builder.Services
    .AddCorsService()
    .AddHttpContextAccessor()
    .AddApiVersioningServices()
    .AddOpenApi(builder)
    .AddHealthChecksServices()
    .AddCaching(configuration)
    .AddAuthenticationServices()
    .AddFeatureManagement()
    .AddUniverse(configuration)
    .AddMassTransit()
    .AddTelemetry()
    .AddRateLimiter()
    .AddSignalR();
*/

builder.Services
    .AddOpenApi(ServiceName, ApiVersions.All, settings =>
    {
        settings
            .AddApiKeySecurity()
            .AddJwtSecurity();
    })
    .AddApiVersioningServices();

builder.Services
    .AddUserContext()
    .AddTenantContext()
    .AddTransient<ExceptionHandlingMiddleware>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumers(typeof(Program).Assembly);
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseTenancyFilters(context);
        cfg.UseIdentityFilters(context);

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseSerilogRequestLogging();

app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseOpenApiAndSwaggerUi();
}

//app.UseCors(CorsExtensions.MyAllowSpecificOrigins);

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

//app.MapControllers();

app.MapApplicationEndpoints();

app.MapHealthChecks("/healthz", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapApplicationHubs();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
}

app.UseRateLimiter();

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    //await context.Database.EnsureDeletedAsync();
    await context.Database.EnsureCreatedAsync();

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