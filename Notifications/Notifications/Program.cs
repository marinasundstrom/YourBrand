using Microsoft.Data.SqlClient;

using Hangfire;
using Hangfire.SqlServer;

using MassTransit;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using NSwag;
using NSwag.Generation.Processors.Security;

using YourBrand.Notifications;
using YourBrand.Notifications.Application;
using YourBrand.Notifications.Infrastructure;
using YourBrand.Notifications.Infrastructure.Persistence;

using Serilog;

using YourBrand;
using YourBrand.Extensions;

static class Program
{
    /// <param name="seed">Seed the database</param>
    /// <param name="args">The rest of the arguments</param>
    /// <returns></returns>
    static async Task Main(bool seed, string? connectionString, string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string ServiceName = "Notifications"
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

        if (args.Contains("--connection-string"))
        {
            builder.Configuration["ConnectionStrings:DefaultConnection"] = args[args.ToList().IndexOf("--connection-string") + 1];
        }

        var Configuration = builder.Configuration;

        var services = builder.Services;

        services.AddApplication(Configuration);
        services.AddInfrastructure(Configuration);
        services.AddServices();

        services
            .AddControllers()
            .AddNewtonsoftJson();

        builder.Services.AddHttpContextAccessor();

        services.AddEndpointsApiExplorer();

        // Register the Swagger services
        services.AddOpenApiDocument(document =>
        {
            document.Title = "Worker API";
            document.Version = "v1";

            document.AddSecurity("JWT", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });

            document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.Authority = "https://localhost:5040";
                        options.Audience = "myapi";

                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            NameClaimType = "name"
                        };

                        //options.TokenValidationParameters.ValidateAudience = false;

                        //options.Audience = "openid";

                        //options.TokenValidationParameters.ValidTypes = new[] { "at+jwt" };
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

        // Add Hangfire services.
        builder.Services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));

        builder.Services.AddHangfireServer();

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

        app.MapHangfireDashboard();

        app.MapGet("/", () => "Hello World!");

        app.MapControllers();

        var configuration = app.Services.GetService<IConfiguration>();

        if (seed)
        {
            await app.Services.SeedAsync();

            using (var connection = new SqlConnection(configuration.GetConnectionString("HangfireConnection", "HangfireDB")))
            {
                connection.Open();

                using (var command = new SqlCommand(string.Format(
                    @"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}') 
                                    create database [{0}];
                      ", "HangfireDB"), connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            return;
        }

        app.Services.InitializeJobs();

        app.Run();
    }
}