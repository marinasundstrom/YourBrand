using System.Text.Json.Serialization;

using MassTransit;

using MediatR;

using YourBrand.Orders.Application;
using YourBrand.Orders.Application.Common.Interfaces;
using YourBrand.Orders.Infrastructure;
using YourBrand.Orders.Infrastructure.Persistence;
using YourBrand.Documents.Client;
using YourBrand.Payments.Client;
using YourBrand.Catalog.Client;
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

builder.Services
    .AddControllers();
    //.AddNewtonsoftJson();

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
    c.Title = "Orders API";
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

builder.Services.AddDocumentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/documents/");
});

builder.Services.AddCatalogClient((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/products/");
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

app.MapControllers();

if(args.Contains("--seed")) 
{
    await SeedData.EnsureSeedData(app);
    return;
}

app.Run();