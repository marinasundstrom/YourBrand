using MassTransit;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;

using Serilog;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Identity;
using YourBrand.IdentityManagement;
using YourBrand.IdentityManagement.Application;
using YourBrand.IdentityManagement.Consumers;
using YourBrand.IdentityManagement.Infrastructure;
using YourBrand.Integration;
using YourBrand.Tenancy;

string MyAllowSpecificOrigins = "MyPolicy";


var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Identity Management";
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

var services = builder.Services;

var configuration = builder.Configuration;

builder.Services
           .AddApplication(builder.Configuration)
           .AddInfrastructure(builder.Configuration)
           .AddServices();

builder.Services
    .AddUserContext()
    .AddTenantContext();


services
    .AddControllers()
    .AddNewtonsoftJson();

services.AddHttpContextAccessor();

services.AddEndpointsApiExplorer();

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
        cfg.UseTenancyFilters(context);
        cfg.UseIdentityFilters(context);

        cfg.ConfigureEndpoints(context);
    });
});


#if DEBUG
IdentityModelEventSource.ShowPII = true;
#endif

services.AddAuthorization();

services.AddAuthenticationServices(configuration);

var app = builder.ConfigureServices();

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

app.MapControllers();

if (args.Contains("--seed"))
{
    await app.EnsureSeedData();

    //await app.Services.SeedAsync();

    return;
}

app.ConfigurePipeline();

app.Run();