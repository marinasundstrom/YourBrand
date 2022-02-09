using System.Data.Common;
using System.Data.SqlClient;

using Contracts;

using Hangfire;
using Hangfire.SqlServer;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using NSwag;
using NSwag.Generation.Processors.Security;

using Worker;
using Worker.Application;
using Worker.Infrastructure;
using Worker.Infrastructure.Persistence;
using Worker.Services;

var builder = WebApplication.CreateBuilder(args);

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
                options.Authority = "https://identity.local";
                options.Audience = "myapi";

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
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
})
.AddMassTransitHostedService()
.AddGenericRequestClient();

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

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseOpenApi();
    app.UseSwaggerUi3(c =>
    {
        c.DocumentTitle = "Worker API v1";
    });
}

//await app.Services.SeedAsync();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHangfireDashboard();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

var configuration = app.Services.GetService<IConfiguration>();

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

app.Services.InitializeJobs();

app.Run();