using YourBrand.Documents.Client;

using YourBrand.RotRutService.Application;
using YourBrand.RotRutService.Application.Common.Interfaces;
using YourBrand.RotRutService.Application.Queries;
using YourBrand.RotRutService.Application.Commands;
using YourBrand.RotRutService.Domain.Enums;
using YourBrand.RotRutService.Infrastructure;
using YourBrand.RotRutService.Services;

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

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerDocument(c =>
{
    c.Title = "RotRutService API";
    c.Version = "0.1";
});

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumers(typeof(Program).Assembly);

    //x.AddRequestClient<IncomingTransactionBatch>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
})
.AddMassTransitHostedService(true)
.AddGenericRequestClient();

builder.Services.AddTransactionsClient((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/transactions/");
});

builder.Services.AddInvoicesClient((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/invoicing/");
});

builder.Services.AddPaymentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/payments/");
});

builder.Services.AddAccountingClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/accounting/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
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