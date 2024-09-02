
using MassTransit;

using Serilog;

using YourBrand;
using YourBrand.ApiKeys;
using YourBrand.ApiKeys.Application;
using YourBrand.ApiKeys.Authentication;
using YourBrand.ApiKeys.Infrastructure;
using YourBrand.ApiKeys.Infrastructure.Persistence;
using YourBrand.Extensions;
using YourBrand.Integration;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "ApiKeys";
string ServiceVersion = "1.0";

// Add services to container

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

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}

builder.Services
    .AddOpenApi(ServiceName, ApiVersions.All, settings =>
    {
        settings
            .AddApiKeySecurity()
            .AddJwtSecurity();
    })
    .AddApiVersioningServices();

builder.Services.AddObservability(ServiceName, ServiceVersion, builder.Configuration);

builder.Services.AddProblemDetails();

var configuration = builder.Configuration;

var services = builder.Services;

services.AddApplication(configuration);
services.AddInfrastructure(configuration);
services.AddServices();

services
    .AddControllers()
    .AddNewtonsoftJson();

builder.Services.AddHttpContextAccessor();

services.AddEndpointsApiExplorer();

services.AddAuthWithJwt();
services.AddAuthWithApiKey();

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

app.MapGet("/", () => "Hello World!");

app.MapControllers();

if (args.Contains("--seed"))
{
    await app.Services.SeedAsync();
    return;
}

app.Run();