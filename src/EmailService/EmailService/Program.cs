using MassTransit;

using Serilog;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.EmailService;
using YourBrand.Extensions;
using YourBrand.Notifications.Consumers;
using YourBrand.Notifications.Services;

using YourBrand.Identity;
using YourBrand.Tenancy;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "EmailService";
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

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    //x.AddConsumers(typeof(Program).Assembly);

    x.AddConsumer<SendEmailConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services
    .AddIdentityServices()
    .AddTenantService();

builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapObservability();

app.MapGet("/", () => "Hello World!");

/* app.MapGet("/greetings", (string name) => new GreetingsResponse($"Greetings, {name}!"))
        .WithName("Greetings_Hi")
        .WithTags("Greetings")
        //.RequireAuthorization()
        .Produces<GreetingsResponse>(StatusCodes.Status200OK); */

app.Run();