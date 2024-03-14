using YourBrand.Documents.Client;

using YourBrand.RotRutService.Application;
using YourBrand.RotRutService.Application.Common.Interfaces;
using YourBrand.RotRutService.Application.Queries;
using YourBrand.RotRutService.Application.Commands;
using YourBrand.RotRutService.Domain.Enums;
using YourBrand.RotRutService.Infrastructure;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Json;
using YourBrand.RotRutService.Infrastructure.Persistence;
using System.Text.Json.Serialization;
using YourBrand.Payments.Client;
using YourBrand.Transactions.Client;
using YourBrand.Accounting.Client;
using YourBrand.Invoicing.Client;
using YourBrand.RotRutService.Domain.Entities;
using YourBrand.RotRutService.Domain;
using YourBrand.Identity;

using Serilog;

using YourBrand;
using YourBrand.Extensions;

using YourBrand.RotRutService;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "RotRutService"
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

if(args.Contains("--connection-string")) 
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] = args[args.ToList().IndexOf("--connection-string") + 1];
}

builder.Services
    .AddApplication()
    .AddDomain()
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

    x.AddConsumers(typeof(Program).Assembly);

    //x.AddRequestClient<IncomingTransactionBatch>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddTransactionsClient((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/transactions/");
});

builder.Services.AddInvoicesClient((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/invoicing/");
});

builder.Services.AddPaymentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/payments/");
});

builder.Services.AddAccountingClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/accounting/");
});

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

app.MapGet("/", () => "Hello World!");

app.MapPost("/BeslutFile", async (RotRut.Beslut.BeslutFil file, IMediator mediator)
    => await mediator.Send(new ReadRotRutResponse(file)))
    .WithName("RotRutService_BeslutFile")
    .WithTags("RotRutService")
    .Produces<string>(StatusCodes.Status200OK);

app.MapGet("/RutFile", async (string? name, IMediator mediator)
    => await mediator.Send(new CreateRutFile(name)))
    .WithName("RotRutService_CreateRutFile")
    .WithTags("RotRutService")
    .Produces<string>(StatusCodes.Status200OK, contentType: "application/xml");

app.MapControllers();

if(args.Contains("--seed")) 
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();