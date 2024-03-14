using YourBrand.Documents.Client;

using YourBrand.Invoicing.Application;
using YourBrand.Invoicing.Application.Common.Interfaces;
using YourBrand.Invoicing.Application.Queries;
using YourBrand.Invoicing.Application.Commands;
using YourBrand.Invoicing.Domain.Enums;
using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Infrastructure;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Json;
using YourBrand.Invoicing.Infrastructure.Persistence;
using System.Text.Json.Serialization;
using YourBrand.Payments.Client;
using YourBrand.Identity;
using Microsoft.AspNetCore.Http.HttpResults;

using Serilog;

using YourBrand;
using YourBrand.Extensions;

using YourBrand.Invoicing;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Invoicing"
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

builder.Services.AddDocumentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/documents/");
});

builder.Services.AddPaymentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/payments/");
});

var app = builder.Build();

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
app.MapGet("/invoices", async (int page, int pageSize, [FromQuery] InvoiceStatus[]? status, IMediator mediator)
    => await mediator.Send(new GetInvoices(page, pageSize, status)))
    .WithName("Invoices_GetInvoices")
    .WithTags("Invoices")
    .Produces<ItemsResult<InvoiceDto>>(StatusCodes.Status200OK);
*/

/*
app.MapGet("/invoices/rut", async (IMediator mediator)
    => await mediator.Send(new CreateRutFile()))
    .WithName("Invoices_CreateRutFile")
    .WithTags("Invoices")
    .Produces<string>(StatusCodes.Status200OK);
*/

app.MapGet("/Invoices/{invoiceId}", async (string invoiceId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetInvoice(invoiceId), cancellationToken))
    .WithName("Invoices_GetInvoice")
    .WithTags("Invoices")
    .Produces<InvoiceDto>(StatusCodes.Status200OK);

app.MapGet("/Invoices/ByNo/{invoiceNo}", async (string invoiceNo, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetInvoiceByNo(invoiceNo), cancellationToken))
    .WithName("Invoices_GetInvoiceByNo")
    .WithTags("Invoices")
    .Produces<InvoiceDto>(StatusCodes.Status200OK);

app.MapPut("/invoices/{invoiceNo}/billingDetails", async Task<Results<Ok, NotFound>> (string invoiceNo, BillingDetailsDto billingDetails, IMediator mediator = default!, CancellationToken cancellationToken = default!) =>
    {
        var result = await mediator.Send(new UpdateBillingDetails(invoiceNo, billingDetails), cancellationToken);

        if (result.HasError(Errors.Invoices.InvoiceNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok();
    })
    .WithName($"Invoices_{nameof(UpdateBillingDetails)}")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/invoices/{invoiceNo}/shippingDetails", async Task<Results<Ok, NotFound>> (string invoiceNo, ShippingDetailsDto shippingDetails, IMediator mediator = default!, CancellationToken cancellationToken = default!) =>
    {
        var result = await mediator.Send(new UpdateShippingDetails(invoiceNo, shippingDetails), cancellationToken);

        if (result.HasError(Errors.Invoices.InvoiceNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok();
    })
    .WithName($"Invoices_{nameof(UpdateShippingDetails)}")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapPost("/invoices/{invoiceNo}/activateRotAndRut", async (string invoiceId, InvoiceDomesticServiceDto dto, IMediator mediator)
    => await mediator.Send(new ActivateRotAndRut(invoiceId, dto)))
    .WithName("Invoices_ActivateRotAndRut")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapGet("/Invoices/{invoiceId}/File", async (string invoiceId, IMediator mediator, CancellationToken cancellationToken)
    => Microsoft.AspNetCore.Http.Results.File(await mediator.Send(new GenerateInvoiceFile(invoiceId), cancellationToken), "application/html", $"{invoiceId}.html"))
    .WithName("Invoices_GetInvoiceFile")
    .WithTags("Invoices")
    .Produces<FileResult>(StatusCodes.Status200OK);

app.MapPost("/Invoices/{invoiceId}/Items", async (string invoiceId, 
    AddInvoiceItem dto, 
    IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new YourBrand.Invoicing.Application.Commands.AddItem(invoiceId, dto.ProductType, dto.Description, dto.UnitPrice, dto.Unit, dto.VatRate, dto.Quantity, dto.IsTaxDeductibleService, dto.DomesticService), cancellationToken))
    .WithName("Invoices_AddItem")
    .WithTags("Invoices")
    .Produces<InvoiceItemDto>(StatusCodes.Status200OK);

app.MapPut("/Invoices/{invoiceId}/Items/{invoiceItemId}", async (string invoiceId, string invoiceItemId,
    UpdateInvoiceItem dto,
    IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new YourBrand.Invoicing.Application.Commands.UpdateInvoiceItem(invoiceId, invoiceItemId, dto.ProductType, dto.Description, dto.UnitPrice, dto.Unit, dto.VatRate, dto.Quantity, dto.IsTaxDeductibleService), cancellationToken))
    .WithName("Invoices_UpdateItem")
    .WithTags("Invoices")
    .Produces<InvoiceItemDto>(StatusCodes.Status200OK);

app.MapPost("/Invoices", async (CreateInvoice command, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(command, cancellationToken))
    .WithName("Invoices_CreateInvoice")
    .WithTags("Invoices")
    .Produces<InvoiceDto>(StatusCodes.Status200OK);

app.MapPut("/Invoices/{invoiceId}/Status", async (string invoiceId, InvoiceStatus status, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetInvoiceStatus(invoiceId, status), cancellationToken))
    .WithName("Invoices_SetInvoiceStatus")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/Invoices/{invoiceId}/PaidAmount", async (string invoiceId, decimal amount, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetPaidAmount(invoiceId, amount), cancellationToken))
    .WithName("Invoices_SetPaidAmount")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/Invoices/{invoiceId}/Date", async (string invoiceId, DateTime date, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetDate(invoiceId, date), cancellationToken))
    .WithName("Invoices_SetDate")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/Invoices/{invoiceId}/Type", async (string invoiceId, InvoiceType type, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetType(invoiceId, type), cancellationToken))
    .WithName("Invoices_SetType")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/Invoices/{invoiceId}/DueDate", async (string invoiceId, DateTime dueDate, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetDueDate(invoiceId, dueDate), cancellationToken))
    .WithName("Invoices_SetDueDate")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/Invoices/{invoiceId}/Reference", async (string invoiceId, string? reference, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetReference(invoiceId, reference), cancellationToken))
    .WithName("Invoices_SetReference")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/Invoices/{invoiceId}/Note", async (string invoiceId, string? note, 
IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetNote(invoiceId, note), cancellationToken))
    .WithName("Invoices_SetNote")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapDelete("/Invoices/{invoiceId}", async (string invoiceId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new DeleteInvoice(invoiceId), cancellationToken))
    .WithName("Invoices_DeleteInvoice")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapDelete("/Invoices/{invoiceId}/Items/{invoiceItemId}", async (string invoiceId, string invoiceItemId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new DeleteInvoiceItem(invoiceId, invoiceItemId), cancellationToken))
    .WithName("Invoices_DeleteInvoiceItem")
    .WithTags("Invoices")
    .Produces(StatusCodes.Status200OK);

app.MapControllers();

if(args.Contains("--seed")) 
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();

public record AddInvoiceItem(ProductType ProductType, string Description, decimal UnitPrice, string Unit, double VatRate, double Quantity, bool? IsTaxDeductibleService, InvoiceItemDomesticServiceDto? DomesticService);

public record UpdateInvoiceItem(ProductType ProductType, string Description, decimal UnitPrice, string Unit, double VatRate, double Quantity, bool IsTaxDeductibleService);