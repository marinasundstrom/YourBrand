using System.Data.SqlClient;

using YourBrand.Accountant;
using YourBrand.Accountant.Consumers;
using YourBrand.Accountant.Services;

using YourBrand.Accounting.Client;

using YourBrand.Documents.Client;

using Hangfire;
using Hangfire.SqlServer;

using YourBrand.Invoices.Client;

using MassTransit;

using YourBrand.Transactions.Client;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;

builder.Services.AddSingleton<IRefundService, RefundService>();
builder.Services.AddSingleton<IReminderService, ReminderService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    //x.AddConsumers(typeof(Program).Assembly);

    x.AddConsumer<InvoicesBatchConsumer>();
    x.AddConsumer<TransactionBatchConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
})
.AddMassTransitHostedService(true)
.AddGenericRequestClient();

builder.Services.AddAccountingClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/accounting/");
});

builder.Services.AddInvoicesClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/invoicing/");
});

builder.Services.AddTransactionsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/transactions/");
});

builder.Services.AddDocumentsClients((sp, http) =>
{
    http.BaseAddress = new Uri($"{Configuration.GetServiceUri("nginx", "https")}/api/documents/");
});

// Add Hangfire services.
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(Configuration.GetConnectionString2("mssql", "HangfireDB"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

/* app.MapGet("/greetings", (string name) => new GreetingsResponse($"Greetings, {name}!"))
        .WithName("Greetings_Hi")
        .WithTags("Greetings")
        //.RequireAuthorization()
        .Produces<GreetingsResponse>(StatusCodes.Status200OK); */

app.UseRouting();

app.MapHangfireDashboard();

using (var connection = new SqlConnection(Configuration.GetConnectionString2("mssql", "HangfireDB")))
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