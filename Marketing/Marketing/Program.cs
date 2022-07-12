using System.Text.Json.Serialization;

using MassTransit;

using MediatR;

using YourBrand.Marketing.Application;
using YourBrand.Marketing.Application.Addresses;
using YourBrand.Marketing.Application.Addresses.Commands;
using YourBrand.Marketing.Application.Addresses.Queries;
using YourBrand.Marketing.Application.Common.Interfaces;
using YourBrand.Marketing.Application.Contacts.Commands;
using YourBrand.Marketing.Application.Contacts.Queries;
using YourBrand.Marketing.Infrastructure;
using YourBrand.Marketing.Infrastructure.Persistence;
using YourBrand.Marketing.Application.Contacts;
using YourBrand.Marketing.Services;
using YourBrand.Documents.Client;
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
    c.Title = "Marketing API";
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

app.MapGet("/Contacts/{personId}", async (string personId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetContact(personId), cancellationToken))
    .WithName("Contacts_GetContacts")
    .WithTags("Contacts")
    .Produces<ContactDto>(StatusCodes.Status200OK);

app.MapDelete("/Contacts/{personId}", async (string personId, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new DeleteContact(personId), cancellationToken))
    .WithName("Contacts_DeleteContact")
    .WithTags("Contacts")
    .Produces(StatusCodes.Status200OK);

app.MapPost("/Contacts", async (CreateContact createContact, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(createContact, cancellationToken))
    .WithName("Contacts_CreateContacts")
    .WithTags("Contacts")
    .Produces<ContactDto>(StatusCodes.Status200OK);

app.MapGet("/Contacts/{personId}/Addresses", async (string personId, string foo, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(new GetAddress(personId), cancellationToken))
    .WithName("Addresses_GetAddress")
    .WithTags("Addresses")
    .Produces<AddressDto>(StatusCodes.Status200OK);

app.MapPost("/Contacts/{personId}/Addresses", async (CreateAddress createAddress, IMediator mediator, CancellationToken cancellationToken)
    => await mediator.Send(createAddress, cancellationToken))
    .WithName("Addresses_CreateAddress")
    .WithTags("Addresses")
    .Produces<AddressDto>(StatusCodes.Status200OK);

app.MapDelete("/Contacts/{personId}/Addresses/{addressId}", async (string personId, string addressId, IMediator mediator, CancellationToken cancellationToken)
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