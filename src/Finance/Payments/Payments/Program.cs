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


builder.AddDefaultOpenApi();


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

builder.Services.AddAuthorization();

builder.Services.AddAuthenticationServices(builder.Configuration);

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

app.UseAuthentication();

app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

/*
app.MapGet("/payments", async (int page, int pageSize, IMediator mediator) => await mediator.Send(new GetPayments(page, pageSize)))
    .WithName("Payments_GetPayments")
    .WithTags("Payments")
    //.RequireAuthorization()
    .Produces<ItemsResult<PaymentDto>>(StatusCodes.Status200OK);
*/

var versionedApi = app.NewVersionedApi("Payments");

var paymentsGroup = versionedApi.MapGroup("/v{version:apiVersion}/Payments")
    .WithTags("Payments")
    .HasApiVersion(ApiVersions.V1)
    .RequireAuthorization()
    .WithOpenApi();

paymentsGroup.MapPost("/", async ([AsParameters] CreatePayment payment, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(payment, cancellationToken))
    .WithName("Payments_CreatePayment")
    .WithTags("Payments")
    //.RequireAuthorization()
    .Produces(StatusCodes.Status200OK);

paymentsGroup.MapGet("/{paymentId}", async (string organizationId, string paymentId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetPaymentById(organizationId, paymentId), cancellationToken))
    .WithName("Payments_GetPaymentById")
    .WithTags("Payments")
    .Produces<PaymentDto>(StatusCodes.Status200OK);

paymentsGroup.MapGet("/GetPaymentByReference/{reference}", async (string organizationId, string reference, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetPaymentByReference(organizationId, reference), cancellationToken))
    .WithName("Payments_GetPaymentByReference")
    .WithTags("Payments")
    .Produces<PaymentDto>(StatusCodes.Status200OK);

paymentsGroup.MapDelete("/{paymentId}", async (string organizationId, string paymentId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new CancelPayment(organizationId, paymentId), cancellationToken))
    .WithName("Payments_CancelPayment")
    .WithTags("Payments")
    .Produces(StatusCodes.Status200OK);

paymentsGroup.MapPut("/{paymentId}/Status", async (string organizationId, string paymentId, PaymentStatus status, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetPaymentStatus(organizationId, paymentId, status), cancellationToken))
    .WithName("Payments_SetPaymentStatus")
    .WithTags("Payments")
    .Produces(StatusCodes.Status200OK);

app.MapHub<PaymentsHub>("/hubs/payments");

app.MapControllers();

if (args.Contains("--seed"))
{
    if (!SeedArguments.TryGetTenantId(args, out var tenantId))
    {
        Console.Error.WriteLine("Tenant id is required when running with --seed. Usage: dotnet run -- --seed -- <tenantId>");
        return;
    }

    await SeedData.EnsureSeedData(app, tenantId);
    return;
}

app.Run();