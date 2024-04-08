using System.Text.Json.Serialization;

using MassTransit;

using MediatR;

using Serilog;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.Documents.Client;
using YourBrand.Extensions;
using YourBrand.Identity;
using YourBrand.Identity;
using YourBrand.Integration;
using YourBrand.Inventory;
using YourBrand.Inventory.Application;
using YourBrand.Inventory.Application.Items.Commands;
using YourBrand.Inventory.Application.Items.Queries;
using YourBrand.Inventory.Infrastructure;
using YourBrand.Inventory.Infrastructure.Persistence;
using YourBrand.Notifications.Client;
using YourBrand.Payments.Client;
using YourBrand.Tenancy;

using ItemDto = YourBrand.Inventory.Application.Items.ItemDto;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Inventory";
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

var configuration = builder.Configuration;

builder.Services
    .AddApplication()
    .AddInfrastructure(configuration);

builder.Services.AddControllers();

// Set the JSON serializer options
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    // options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddUserContext()
    .AddTenantContext();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    //x.AddConsumers(typeof(Program).Assembly);

    //x.AddRequestClient<IncomingTransactionBatch>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseTenancyFilters(context);
        cfg.UseIdentityFilters(context);

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddAuthorization();

builder.Services.AddAuthenticationServices(configuration);

//builder.Services.AddApiKeyAuthentication("https://localhost:5174/api/apikeys/");

builder.Services.AddDocumentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/documents/");
});

builder.Services.AddPaymentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/payments/");
});

builder.Services.AddNotificationsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/notifications/");
}, b => { });

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapObservability();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApiAndSwaggerUi();
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

app.MapGet("/Items/{personId}", async (string personId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetItem(personId), cancellationToken))
    .WithName("Items_GetItems")
    .WithTags("Items")
    .Produces<ItemDto>(StatusCodes.Status200OK);

app.MapDelete("/Items/{personId}", async (string personId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new DeleteItem(personId), cancellationToken))
    .WithName("Items_DeleteItem")
    .WithTags("Items")
    .Produces(StatusCodes.Status200OK);

app.MapPost("/Items", async (AddItem createItem, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(createItem, cancellationToken))
    .WithName("Items_CreateItems")
    .WithTags("Items")
    .Produces<ItemDto>(StatusCodes.Status200OK);

app.MapControllers();

if (args.Contains("--seed"))
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();