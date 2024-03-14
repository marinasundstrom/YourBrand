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

        services.AddAuthorization();

        services.AddAuthenticationServices(Configuration);

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

        app.MapControllers();

        if (seed)
        {
            await app.Services.SeedAsync();

            return;
        }

        app.Run();
    }
}