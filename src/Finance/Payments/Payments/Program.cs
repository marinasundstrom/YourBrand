using System.Text.Json.Serialization;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Http.Json;

using Serilog;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Identity;
using YourBrand.Integration;
using YourBrand.Payments;
using YourBrand.Payments.Application;
using YourBrand.Payments.Application.Commands;
using YourBrand.Payments.Application.Queries;
using YourBrand.Payments.Application.Services;
using YourBrand.Payments.Domain.Enums;
using YourBrand.Payments.Hubs;
using YourBrand.Payments.Infrastructure;
using YourBrand.Payments.Infrastructure.Persistence;
using YourBrand.Tenancy;
using YourBrand.Transactions.Client;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Payments";
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

builder.AddServiceDefaults();

/*
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDiscoveryClient();
}
*/

builder.Services
    .AddOpenApi(ServiceName, ApiVersions.All, settings => settings.AddJwtSecurity())
    .AddApiVersioningServices();

//builder.Services.AddObservability(ServiceName, ServiceVersion, builder.Configuration);

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

builder.Services.AddScoped<IPaymentsHubClient, PaymentsHubClient>();

builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddUserContext()
    .AddTenantContext();

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    // options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransactionsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/transactions/");
});

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumers(typeof(Program).Assembly);

    //x.AddRequestClient<YourBrand.Payments.Contracts.PaymentBatch>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.UseTenancyFilters(context);
        cfg.UseIdentityFilters(context);

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseSerilogRequestLogging();

//app.MapObservability();

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

app.MapGet("/", () => "Hello World!");

/*
app.MapGet("/payments", async (int page, int pageSize, IMediator mediator) => await mediator.Send(new GetPayments(page, pageSize)))
    .WithName("Payments_GetPayments")
    .WithTags("Payments")
    //.RequireAuthorization()
    .Produces<ItemsResult<PaymentDto>>(StatusCodes.Status200OK);
*/

app.MapPost("/Payments", async (CreatePayment payment, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(payment, cancellationToken))
    .WithName("Payments_CreatePayment")
    .WithTags("Payments")
    //.RequireAuthorization()
    .Produces(StatusCodes.Status200OK);

app.MapGet("/Payments/{paymentId}", async (string paymentId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetPaymentById(paymentId), cancellationToken))
    .WithName("Payments_GetPaymentById")
    .WithTags("Payments")
    .Produces<PaymentDto>(StatusCodes.Status200OK);

app.MapGet("/Payments/GetPaymentByReference/{reference}", async (string reference, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetPaymentByReference(reference), cancellationToken))
    .WithName("Payments_GetPaymentByReference")
    .WithTags("Payments")
    .Produces<PaymentDto>(StatusCodes.Status200OK);

app.MapDelete("/Payments/{paymentId}", async (string paymentId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new CancelPayment(paymentId), cancellationToken))
    .WithName("Payments_CancelPayment")
    .WithTags("Payments")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/Payments/{paymentId}/Status", async (string paymentId, PaymentStatus status, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetPaymentStatus(paymentId, status), cancellationToken))
    .WithName("Payments_SetPaymentStatus")
    .WithTags("Payments")
    .Produces(StatusCodes.Status200OK);

app.MapHub<PaymentsHub>("/hubs/payments");

app.MapControllers();

if (args.Contains("--seed"))
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();