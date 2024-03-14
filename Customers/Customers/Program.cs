using System.Text.Json.Serialization;

using MassTransit;

using MediatR;

using YourBrand.Customers.Application;
using YourBrand.Customers.Application.Addresses;
using YourBrand.Customers.Application.Addresses.Commands;
using YourBrand.Customers.Application.Addresses.Queries;
using YourBrand.Customers.Application.Common.Interfaces;
using YourBrand.Customers.Application.Persons.Commands;
using YourBrand.Customers.Application.Persons.Queries;
using YourBrand.Customers.Infrastructure;
using YourBrand.Customers.Infrastructure.Persistence;
using YourBrand.Customers.Application.Persons;
using YourBrand.Documents.Client;
using YourBrand.Payments.Client;
using YourBrand.Identity;

using Serilog;

using YourBrand;
using YourBrand.Extensions;
using YourBrand.Customers;

var builder = WebApplication.CreateBuilder(args);

string ServiceName = "Customers"
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

app.MapGet("/Persons/{personId}", async (string personId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetPerson(personId), cancellationToken))
    .WithName("Persons_GetPersons")
    .WithTags("Persons")
    .Produces<PersonDto>(StatusCodes.Status200OK);

app.MapDelete("/Persons/{personId}", async (string personId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new DeletePerson(personId), cancellationToken))
    .WithName("Persons_DeletePerson")
    .WithTags("Persons")
    .Produces(StatusCodes.Status200OK);

app.MapPost("/Persons", async (CreatePerson createPerson, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(createPerson, cancellationToken))
    .WithName("Persons_CreatePersons")
    .WithTags("Persons")
    .Produces<PersonDto>(StatusCodes.Status200OK);

app.MapGet("/Persons/{personId}/Addresses", async (string personId, string foo, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetAddress(personId), cancellationToken))
    .WithName("Addresses_GetAddress")
    .WithTags("Addresses")
    .Produces<AddressDto>(StatusCodes.Status200OK);

app.MapPost("/Persons/{personId}/Addresses", async (CreateAddress createAddress, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(createAddress, cancellationToken))
    .WithName("Addresses_CreateAddress")
    .WithTags("Addresses")
    .Produces<AddressDto>(StatusCodes.Status200OK);

app.MapDelete("/Persons/{personId}/Addresses/{addressId}", async (string personId, string addressId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new DeleteAddress(addressId), cancellationToken))
    .WithName("Addresses_DeleteAddress")
    .WithTags("Addresses")
    .Produces(StatusCodes.Status200OK);

app.MapControllers();

if(args.Contains("--seed")) 
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();