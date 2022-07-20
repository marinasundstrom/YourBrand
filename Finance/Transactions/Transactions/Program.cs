using System.Text.Json.Serialization;

using MassTransit;

using MediatR;

using Microsoft.AspNetCore.Http.Json;

using YourBrand.Identity;
using YourBrand.Transactions.Application;
using YourBrand.Transactions.Application.Commands;
using YourBrand.Transactions.Application.Common.Interfaces;
using YourBrand.Transactions.Application.Queries;
using YourBrand.Transactions.Application.Services;
using YourBrand.Transactions.Domain.Enums;
using YourBrand.Transactions.Hubs;
using YourBrand.Transactions.Infrastructure;
using YourBrand.Transactions.Infrastructure.Persistence;

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

builder.Services.AddScoped<ITransactionsHubClient, TransactionsHubClient>();

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
    c.Title = "Transactions API";
    c.Version = "0.1";
});

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    //x.AddConsumers(typeof(Program).Assembly);

    x.AddRequestClient<YourBrand.Transactions.Contracts.IncomingTransactionBatch>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
})
.AddMassTransitHostedService(true)
.AddGenericRequestClient();

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
app.MapGet("/transactions", async (int page, int pageSize, IMediator mediator) => await mediator.Send(new GetPayments(page, pageSize)))
    .WithName("Transactions_GetTransactions")
    .WithTags("Transactions")
    //.RequireAuthorization()
    .Produces<ItemsResult<TransactionDto>>(StatusCodes.Status200OK);
*/

app.MapPost("/transactions", async (TransactionDto[] transactions, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new PostTransactions(transactions), cancellationToken))
    .WithName("Transactions_PostTransactions")
    .WithTags("Transactions")
    //.RequireAuthorization()
    .Produces(StatusCodes.Status200OK); ;

app.MapPut("/transactions/{transactionId}/status", async (string transactionId, TransactionStatus status, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new SetTransactionStatus(transactionId, status), cancellationToken))
    .WithName("Transactions_SetTransactionStatus")
    .WithTags("Transactions")
    .Produces(StatusCodes.Status200OK);

app.MapPut("/transactions/{transactionId}/reference", async (string transactionId, string reference, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new UpdateTransactionReference(transactionId, reference), cancellationToken))
    .WithName("Transactions_SetTransactionReference")
    .WithTags("Transactions")
    .Produces(StatusCodes.Status200OK);

app.MapHub<TransactionsHub>("/hubs/transactions");

app.MapControllers();

if(args.Contains("--seed")) 
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();