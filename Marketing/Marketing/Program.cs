using System.Text.Json.Serialization;

using MassTransit;

using MediatR;

using YourBrand.Marketing.Application;
using YourBrand.Marketing.Application.Addresses;
using YourBrand.Marketing.Application.Addresses.Commands;
using YourBrand.Marketing.Application.Addresses.Queries;
using YourBrand.Marketing.Application.Common.Interfaces;
using YourBrand.Marketing.Application.Contacts.Commands;
using YourBrand.Marketing.Application.Contacts.Queries;
using YourBrand.Marketing.Infrastructure;
using YourBrand.Marketing.Infrastructure.Persistence;
using YourBrand.Marketing.Application.Contacts;
using YourBrand.Documents.Client;
using YourBrand.Payments.Client;
using YourBrand.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

using Serilog;

using YourBrand;
using YourBrand.Extensions;

using YourBrand.Marketing;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Marketing"
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
var Configuration = builder.Configuration;

if (args.Contains("--connection-string"))
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] = args[args.ToList().IndexOf("--connection-string") + 1];
}

builder.Services
    .AddApplication()
    .AddInfrastructure(Configuration);

builder.Services.AddControllers();

// Set the JSON serializer options
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    // options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentityServices();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    //x.AddConsumers(typeof(Program).Assembly);

    //x.AddRequestClient<IncomingTransactionBatch>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                   .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                   {
                       options.Authority = "https://localhost:5040";
                       options.Audience = "myapi";

                       options.TokenValidationParameters = new TokenValidationParameters()
                       {
                           NameClaimType = "name"

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


builder.Services.AddDocumentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/documents/");
});

builder.Services.AddPaymentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/payments/");
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapObservability();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

if (args.Contains("--seed"))
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();