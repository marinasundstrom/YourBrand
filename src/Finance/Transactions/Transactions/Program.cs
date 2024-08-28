using System.Text.Json.Serialization;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Http.Json;

using Serilog;

using Steeltoe.Discovery.Client;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Identity;
using YourBrand.Identity;
using YourBrand.Integration;
using YourBrand.Tenancy;
using YourBrand.Transactions;
using YourBrand.Transactions.Application;
using YourBrand.Transactions.Application.Commands;
using YourBrand.Transactions.Application.Services;
using YourBrand.Transactions.Domain.Enums;
using YourBrand.Transactions.Hubs;
using YourBrand.Transactions.Infrastructure;
using YourBrand.Transactions.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Transactions";
string ServiceVersion = "1.0";

// Add services to container

builder.Host.UseSerilog((ctx, cfg) => { 
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

builder.Services
    .AddUserContext()
    .AddTenantContext();

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

builder.Services.AddScoped<ITransactionsHubClient, TransactionsHubClient>();

builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();

builder.Services.AddUserContext();

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    // options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    //x.AddConsumers(typeof(Program).Assembly);

    x.AddRequestClient<YourBrand.Transactions.Contracts.IncomingTransactionBatch>();

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
app.MapGet("/transactions", async (int page, int pageSize, IMediator mediator) => await mediator.Send(new GetPayments(page, pageSize)))
    .WithName("Transactions_GetTransactions")
    .WithTags("Transactions")
    //.RequireAuthorization()
    .Produces<ItemsResult<TransactionDto>>(StatusCodes.Status200OK);
*/

var versionedApi = app.NewVersionedApi("Transactions");

var group = versionedApi.MapGroup("/v{version:apiVersion}/transactions")
    .WithTags("Transactions")
    .HasApiVersion(ApiVersions.V1)
    .WithOpenApi();

group.MapPost("/", async (TransactionDto[] transactions, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new PostTransactions(transactions), cancellationToken))
    .WithName("Transactions_PostTransactions")
    //.RequireAuthorization()
    .Produces(StatusCodes.Status200OK); ;

group.MapPut("/{transactionId}/status", async (string transactionId, TransactionStatus status, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetTransactionStatus(transactionId, status), cancellationToken))
    .WithName("Transactions_SetTransactionStatus")
    .Produces(StatusCodes.Status200OK);

group.MapPut("/{transactionId}/reference", async (string transactionId, string reference, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new UpdateTransactionReference(transactionId, reference), cancellationToken))
    .WithName("Transactions_SetTransactionReference")
    .Produces(StatusCodes.Status200OK);

app.MapHub<TransactionsHub>("/hubs/transactions");

app.MapControllers();

if (args.Contains("--seed"))
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();