using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Azure.Identity;
using Azure.Storage.Blobs;

using Catalog.Application;
using Catalog.Application;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Persistence;
using Catalog.WebApi;
using Catalog.WebApi.Hubs;

using MassTransit;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

using NSwag;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var Configuration = builder.Configuration;

services.AddApplication(Configuration);
services.AddInfrastructure(Configuration);
services.AddServices();

services
    .AddControllers()
    .AddNewtonsoftJson();

services.AddHttpContextAccessor();

services.AddEndpointsApiExplorer();

// Register the Swagger services
services.AddOpenApiDocument(document =>
{
    document.Title = "Web API";
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

services.AddAzureClients(builder =>
{
    // Add a KeyVault client
    //builder.AddSecretClient(keyVaultUrl);

    // Add a Storage account client
    builder.AddBlobServiceClient(Configuration.GetConnectionString("Azure:Storage"))
            .WithVersion(BlobClientOptions.ServiceVersion.V2019_07_07);

    // Use DefaultAzureCredential by default
    builder.UseCredential(new DefaultAzureCredential());
});

services.AddSignalR();

services.AddSingleton<IUserIdProvider, EmailBasedUserIdProvider>();

services.AddMassTransit(x =>
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

services.AddStackExchangeRedisCache(o =>
{
    o.Configuration = Configuration.GetConnectionString("redis");
});

#if DEBUG
IdentityModelEventSource.ShowPII = true;
#endif

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

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Add the access_token as a claim, as we may actually need it
                        var accessToken = context.SecurityToken as JwtSecurityToken;
                        if (accessToken != null)
                        {
                            ClaimsIdentity? identity = context.Principal.Identity as ClaimsIdentity;
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseOpenApi();
    app.UseSwaggerUi3(c =>
    {
        c.DocumentTitle = "Web API v1";
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapApplicationRequests();

app.MapGet("/info", () =>
{
    return System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString();
})
.WithDisplayName("GetInfo")
.WithName("GetInfo")
.WithTags("Info")
.Produces<string>();

//await app.Services.SeedAsync();

app.MapControllers();
app.MapHub<ItemsHub>("/hubs/items");
app.MapHub<SomethingHub>("/hubs/something");
app.MapHub<WorkerHub>("/hubs/worker");
app.MapHub<NotificationHub>("/hubs/notifications");

app.Run();