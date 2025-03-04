using HealthChecks.UI.Client;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using NJsonSchema;
using NJsonSchema.Generation.TypeMappers;

using Serilog;

using YourBrand.ChatApp;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.ChatApp.Infrastructure.Persistence;
using YourBrand.ChatApp.Web.Extensions;
using YourBrand.ChatApp.Web.Middleware;
using YourBrand.Domain;
using YourBrand.Extensions;
using YourBrand.Integration;
using YourBrand.Tenancy;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "ChatApp";
string ServiceVersion = "1.0";

var configuration = builder.Configuration;

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(builder.Configuration)
    .WriteTo.OpenTelemetry(options =>
    {
        options.Endpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        var headers = builder.Configuration["OTEL_EXPORTER_OTLP_HEADERS"]?.Split(',') ?? [];
        foreach (var header in headers)
        {
            var (key, value) = header.Split('=') switch
            {
            [string k, string v] => (k, v),
                var v => throw new Exception($"Invalid header format {v}")
            };

            options.Headers.Add(key, value);
        }
        options.ResourceAttributes.Add("service.name", ServiceName);
    })
    .Enrich.WithProperty("Application", ServiceName)
    .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName);
});

builder.AddServiceDefaults();

builder.Services.AddProblemDetails();

builder.Services.AddAuthorization();

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddControllers();

builder.Services
    .AddUniverse(configuration)
    .AddRateLimiter2()
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
    .AddOpenApi(ServiceName, "", ApiVersions.All, settings =>
    {
        settings.SchemaSettings.TypeMappers.Add(new ObjectTypeMapper(typeof(ChannelId), new NJsonSchema.JsonSchema
        {
            Type = JsonObjectType.String
        }));

        settings.SchemaSettings.TypeMappers.Add(new ObjectTypeMapper(typeof(ChannelParticipantId), new NJsonSchema.JsonSchema
        {
            Type = JsonObjectType.String
        }));

        settings.SchemaSettings.TypeMappers.Add(new ObjectTypeMapper(typeof(MessageId), new NJsonSchema.JsonSchema
        {
            Type = JsonObjectType.String
        }));

        settings.SchemaSettings.TypeMappers.Add(new ObjectTypeMapper(typeof(OrganizationId), new NJsonSchema.JsonSchema
        {
            Type = JsonObjectType.String
        }));

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

app.UseRateLimiter();

using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{

    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    var dbProviderName = context.Database.ProviderName;

    if (dbProviderName!.Contains("SqlServer"))
    {
        //await context.Database.EnsureCreatedAsync();

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
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

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