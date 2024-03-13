using Microsoft.Data.SqlClient;

using YourBrand.Accountant;
using YourBrand.Accountant.Consumers;
using YourBrand.Accountant.Services;

using YourBrand.Accounting.Client;

using YourBrand.Documents.Client;

using Hangfire;
using Hangfire.SqlServer;

using YourBrand.Invoicing.Client;

using MassTransit;
using YourBrand.Payments.Client;
using YourBrand.Accountant.Domain;

using Serilog;

using YourBrand;
using YourBrand.Extensions;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

string ServiceName = "Accountant"
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


builder.Services.AddDomain();

builder.Services.AddSingleton<IRefundService, RefundService>();
builder.Services.AddSingleton<IReminderService, ReminderService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    //x.AddConsumers(typeof(Program).Assembly);

    x.AddConsumer<InvoicesBatchConsumer>();
    x.AddConsumer<PaymentCapturedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddAccountingClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/accounting/");
});

builder.Services.AddInvoicingClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/invoicing/");
});

builder.Services.AddPaymentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/payments/");
});

builder.Services.AddDocumentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"https://localhost:5174/api/documents/");
});

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.MapObservability();

app.MapGet("/", () => "Hello World!");

/* app.MapGet("/greetings", (string name) => new GreetingsResponse($"Greetings, {name}!"))
        .WithName("Greetings_Hi")
        .WithTags("Greetings")
        //.RequireAuthorization()
        .Produces<GreetingsResponse>(StatusCodes.Status200OK); */

app.UseRouting();

app.MapHangfireDashboard();

using (var connection = new SqlConnection(Configuration.GetConnectionString("HangfireConnection")))
{
    connection.Open();

    using (var command = new SqlCommand(string.Format(
        @"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}') 
                                    create database [{0}];
                      ", "HangfireDB"), connection))
    {
        command.ExecuteNonQuery();
    }
}

app.Services.InitializeJobs();

app.Run();