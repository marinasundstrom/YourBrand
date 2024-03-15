using YourBrand.IdentityService;

using Serilog;
using YourBrand.IdentityService.Infrastructure.Persistence;

using Serilog;

using YourBrand;
using YourBrand.Extensions;
using Steeltoe.Discovery.Client;

/* Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();*/

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    string ServiceName = "IdentityService";
    string ServiceVersion = "1.0";

    // Add services to container

    builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(builder.Configuration)
                            .Enrich.WithProperty("Application", ServiceName)
                            .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName));

    if (builder.Environment.IsDevelopment())
    {
        builder.Services.AddDiscoveryClient();
    }

    builder.Services
        .AddOpenApi(ServiceName, ApiVersions.All)
        .AddApiVersioningServices();

    builder.Services.AddObservability(ServiceName, ServiceVersion, builder.Configuration);

    builder.Services.AddProblemDetails();

    var app = builder
        .ConfigureServices()
        .ConfigurePipeline();

    app.UseSerilogRequestLogging();

    app.MapObservability();

    if (app.Environment.IsDevelopment())
    {
        app.UseOpenApiAndSwaggerUi();
    }

    // this seeding is only for the template to bootstrap the DB and users.
    // in production you will likely want a different approach.
    if (args.Contains("--seed"))
    {
        Log.Information("Seeding database...");
        await SeedData.EnsureSeedData(app.Services);
        Log.Information("Done seeding database. Exiting.");

        return;
    }

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException") // https://github.com/dotnet/runtime/issues/60600
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Shut down complete");
    Log.CloseAndFlush();

}