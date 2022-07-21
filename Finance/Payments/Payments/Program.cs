using System.Text.Json.Serialization;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Http.Json;

using YourBrand.Identity;
using YourBrand.Payments.Application;
using YourBrand.Payments.Application.Commands;
using YourBrand.Payments.Application.Common.Interfaces;
using YourBrand.Payments.Application.Queries;
using YourBrand.Payments.Application.Services;
using YourBrand.Payments.Domain.Enums;
using YourBrand.Payments.Hubs;
using YourBrand.Payments.Infrastructure;
using YourBrand.Payments.Infrastructure.Persistence;
using YourBrand.Transactions.Client;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

if(args.Contains("--connection-string")) 
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] = args[args.ToList().IndexOf("--connection-string") + 1];
}

Console.WriteLine(builder.Configuration["ConnectionStrings:DefaultConnection"]);

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

builder.Services.AddScoped<IPaymentsHubClient, PaymentsHubClient>();

builder.Services.AddSignalR();

builder.Services.AddHttpContextAccessor();

builder.Services.AddIdentityServices();

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    // options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerDocument(c =>
{
    c.Title = "Payments API";
    c.Version = "0.1";
});

builder.Services.AddTransactionsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/transactions/");
});

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumers(typeof(Program).Assembly);

    //x.AddRequestClient<YourBrand.Payments.Contracts.PaymentBatch>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
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

if(args.Contains("--seed")) 
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();