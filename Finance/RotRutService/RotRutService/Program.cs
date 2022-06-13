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

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

if(args.Contains("--connection-string")) 
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

    //x.AddConsumers(typeof(Program).Assembly);

    //x.AddRequestClient<IncomingTransactionBatch>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
})
.AddMassTransitHostedService(true)
.AddGenericRequestClient();

builder.Services.AddDocumentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/documents/");
});

builder.Services.AddPaymentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/payments/");
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

/*
app.MapGet("/invoices", async (int page, int pageSize, [FromQuery] InvoiceStatus[]? status, IMediator mediator)
    => await mediator.Send(new GetInvoices(page, pageSize, status)))
    .WithName("Invoices_GetInvoices")
    .WithTags("RotRutService")
    .Produces<ItemsResult<InvoiceDto>>(StatusCodes.Status200OK);
*/

/*

app.MapGet("/invoices/rut", async (IMediator mediator)
    => await mediator.Send(new CreateRutFile()))
    .WithName("Invoices_CreateRutFile")
    .WithTags("RotRutService")
    .Produces<string>(StatusCodes.Status200OK);

app.MapGet("/RotRutService/{invoiceId}", async (int invoiceId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetInvoice(invoiceId), cancellationToken))
    .WithName("Invoices_GetInvoice")
    .WithTags("RotRutService")
    .Produces<InvoiceDto>(StatusCodes.Status200OK);

app.MapPost("/invoices/activateRotAndRut", async (int invoiceId, InvoiceDomesticServiceDto dto, IMediator mediator)
    => await mediator.Send(new ActivateRotAndRut(invoiceId, dto)))
    .WithName("Invoices_ActivateRotAndRut")
    .WithTags("RotRutService")
    .Produces<string>(StatusCodes.Status200OK);

app.MapGet("/RotRutService/{invoiceId}/File", async (int invoiceId, IMediator mediator, CancellationToken cancellationToken)
    => Results.File(await mediator.Send(new GenerateInvoiceFile(invoiceId), cancellationToken), "application/html", $"{invoiceId}.html"))
    .WithName("Invoices_GetInvoiceFile")
    .WithTags("RotRutService")
    .Produces<FileResult>(StatusCodes.Status200OK);

app.MapPost("/RotRutService/{invoiceId}/Items", async (int invoiceId, 
    AddItemItem dto, 
    IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new YourBrand.RotRutService.Application.Commands.AddItemItem(invoiceId, dto.ProductType, dto.Description, dto.UnitPrice, dto.Unit, dto.VatRate, dto.Quantity, dto.DomesticService), cancellationToken))
    .WithName("Invoices_AddItem")
    .WithTags("RotRutService")
    .Produces<InvoiceItemDto>(StatusCodes.Status200OK);

app.MapPost("/RotRutService", async (CreateInvoice command, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(command, cancellationToken))
    .WithName("Invoices_CreateInvoice")
    .WithTags("RotRutService")
    .Produces<InvoiceDto>(StatusCodes.Status200OK);

app.MapPut("/RotRutService/{invoiceId}/Status", async (int invoiceId, InvoiceStatus status, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetInvoiceStatus(invoiceId, status), cancellationToken))
    .WithName("Invoices_SetInvoiceStatus")
    .WithTags("RotRutService")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/RotRutService/{invoiceId}/PaidAmount", async (int invoiceId, decimal amount, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetPaidAmount(invoiceId, amount), cancellationToken))
    .WithName("Invoices_SetPaidAmount")
    .WithTags("RotRutService")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/RotRutService/{invoiceId}/Date", async (int invoiceId, DateTime date, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetDate(invoiceId, date), cancellationToken))
    .WithName("Invoices_SetDate")
    .WithTags("RotRutService")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/RotRutService/{invoiceId}/Type", async (int invoiceId, InvoiceType type, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetType(invoiceId, type), cancellationToken))
    .WithName("Invoices_SetType")
    .WithTags("RotRutService")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/RotRutService/{invoiceId}/DueDate", async (int invoiceId, DateTime dueDate, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetDueDate(invoiceId, dueDate), cancellationToken))
    .WithName("Invoices_SetDueDate")
    .WithTags("RotRutService")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/RotRutService/{invoiceId}/Reference", async (int invoiceId, string? reference, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetReference(invoiceId, reference), cancellationToken))
    .WithName("Invoices_SetReference")
    .WithTags("RotRutService")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/RotRutService/{invoiceId}/Note", async (int invoiceId, string? note, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetNote(invoiceId, note), cancellationToken))
    .WithName("Invoices_SetNote")
    .WithTags("RotRutService")
    .Produces(StatusCodes.Status200OK);

app.MapDelete("/RotRutService/{invoiceId}", async (int invoiceId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new DeleteInvoice(invoiceId), cancellationToken))
    .WithName("Invoices_DeleteInvoice")
    .WithTags("RotRutService")
    .Produces(StatusCodes.Status200OK);

app.MapDelete("/RotRutService/{invoiceId}/Items/{invoiceItemId}", async (int invoiceId, int invoiceItemId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new DeleteInvoiceItem(invoiceId, invoiceItemId), cancellationToken))
    .WithName("Invoices_DeleteInvoiceItem")
    .WithTags("RotRutService")
    .Produces(StatusCodes.Status200OK);

*/

app.MapControllers();

if(args.Contains("--seed")) 
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();