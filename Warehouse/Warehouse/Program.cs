using System.Text.Json.Serialization;

using MassTransit;

using MediatR;

using YourBrand.Warehouse.Application;
using YourBrand.Warehouse.Application.Common.Interfaces;
using YourBrand.Warehouse.Application.Items.Commands;
using YourBrand.Warehouse.Application.Items.Queries;
using YourBrand.Warehouse.Infrastructure;
using YourBrand.Warehouse.Infrastructure.Persistence;
using YourBrand.Warehouse.Application.Items;
using YourBrand.Documents.Client;
using YourBrand.Payments.Client;
using YourBrand.Identity;

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

builder.Services.AddIdentityServices();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerDocument(c =>
{
    c.Title = "Warehouse API";
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
});

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

app.MapPost("/Items", async (CreateItem createItem, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(createItem, cancellationToken))
    .WithName("Items_CreateItems")
    .WithTags("Items")
    .Produces<ItemDto>(StatusCodes.Status200OK);

app.MapControllers();

if(args.Contains("--seed")) 
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();