using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("messaging-username", true);
var password = builder.AddParameter("messaging-password", true);

var messaging = builder.AddRabbitMQ("messaging", userName: username, password: password, port: 5672)
    .WithHttpEndpoint(port: 15672, targetPort: 15672, name: "management")
    .WithBindMount("../../data/rabbitmq", "/var/lib/rabbitmq");

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(container =>
                   {
                       container.UseBlobPort(10000);
                       container.WithBindMount("../../data/azurite", "/data");
                   });

var blobStorage = storage.AddBlobs("blobs");

/*
var customContainer = builder.AddContainer("consul-server", "hashicorp/consul", "latest")
                             .WithHttpEndpoint(port: 8500, targetPort: 8500, name: "endpoint")
                             .WithEndpoint(port: 8600, targetPort: 8600, name: "tcp");

var endpoint = customContainer.GetEndpoint("endpoint");
*/

var sqlServerPassword = builder.AddParameter("sqlServerPassword", true);

var sqlServer = builder.AddSqlServer("sql", password: sqlServerPassword, port: 1433)
    .WithBindMount("../../data/sql-edge", "/var/opt/mssql")
    .WithEnvironment("MSSQL_PID", "Developer")
    .WithHealthCheck();

var hangfireDb = sqlServer.AddDatabase("hangfireDb", "HangfireDB");

var portal = builder.AddProject<Portal>("portal");

var appserviceDb = sqlServer.AddDatabase("appserviceDb", "AppService");
var appservice = builder.AddProject<AppService>("appservice")
    .WithReference(appserviceDb)
    .WithReference(messaging)
    .WithReference(blobStorage);

var identityManagementDb = sqlServer.AddDatabase("identityManagementDb", "IdentityManagement");
var identityManagement = builder.AddProject<IdentityManagement>("identityManagement")
    .WithReference(identityManagementDb)
    .WithReference(messaging)
    .WithReference(blobStorage);

var notificationsDb = sqlServer.AddDatabase("notificationsDb", "Notifications");
var notifications = builder.AddProject<Notifications>("notifications")
    .WithReference(notificationsDb)
    .WithReference(hangfireDb)
    .WithReference(messaging)
    .WithReference(blobStorage)
    .WaitOn(hangfireDb);

var salesDb = sqlServer.AddDatabase("salesDb", "Sales");
var sales = builder.AddProject<Sales>("sales")
    .WithReference(salesDb)
    .WithReference(messaging)
    .WithReference(blobStorage);

var catalogDb = sqlServer.AddDatabase("catalogDb", "Catalog");
var catalog = builder.AddProject<Catalog>("catalog")
    .WithReference(catalogDb)
    .WithReference(messaging)
    .WithReference(blobStorage);

var customersDb = sqlServer.AddDatabase("customersDb", "Customers");
var customers = builder.AddProject<Customers>("customers")
    .WithReference(customersDb)
    .WithReference(messaging)
    .WithReference(blobStorage);

var ticketingDb = sqlServer.AddDatabase("ticketingDb", "Ticketing");
var ticketing = builder.AddProject<Ticketing>("ticketing")
    .WithReference(ticketingDb)
    .WithReference(messaging)
    .WithReference(blobStorage);

var humanresourcesDb = sqlServer.AddDatabase("humanresourcesDb", "HumanResources");
var humanresources = builder.AddProject<HumanResources>("humanresources")
    .WithReference(humanresourcesDb)
    .WithReference(messaging)
    .WithReference(blobStorage);

var marketingDb = sqlServer.AddDatabase("marketingDb", "Marketing");
var marketing = builder.AddProject<Marketing>("marketing")
    .WithReference(marketingDb)
    .WithReference(messaging)
    .WithReference(blobStorage);

var inventoryDb = sqlServer.AddDatabase("inventoryDb", "Inventory");
var inventory = builder.AddProject<Inventory>("inventory")
.WithReference(inventoryDb)
.WithReference(messaging)
.WithReference(blobStorage);

//

var identityserviceDb = sqlServer.AddDatabase("identityserviceDb", "AspIdUsers");
var identityService = builder.AddProject<IdentityService>("identityservice")
.WithReference(identityserviceDb)
.WithReference(messaging)
.WithReference(blobStorage);

//

var timereportDb = sqlServer.AddDatabase("timereportDb", "TimeReport");
var timereport = builder.AddProject<TimeReport>("timereport")
.WithReference(timereportDb)
.WithReference(messaging)
.WithReference(blobStorage);

var showroomDb = sqlServer.AddDatabase("showroomDb", "Showroom");
var showroom = builder.AddProject<Showroom>("showroom")
.WithReference(showroomDb)
.WithReference(messaging)
.WithReference(blobStorage);

var accountingDb = sqlServer.AddDatabase("accountingDb", "Accounting");
var accounting = builder.AddProject<Accounting>("accounting")
.WithReference(accountingDb)
.WithReference(messaging)
.WithReference(blobStorage);

var transactionsDb = sqlServer.AddDatabase("transactionsDb", "Transactions");
var transactions = builder.AddProject<Transactions>("transactions")
.WithReference(transactionsDb)
.WithReference(messaging)
.WithReference(blobStorage);

var paymentsDb = sqlServer.AddDatabase("paymentsDb", "Payments");
var payments = builder.AddProject<Payments>("payments")
.WithReference(paymentsDb)
.WithReference(messaging)
.WithReference(blobStorage);

var invoicingDb = sqlServer.AddDatabase("invoicingDb", "Invoicing");
var invoicing = builder.AddProject<Invoicing>("invoicing")
.WithReference(invoicingDb)
.WithReference(messaging)
.WithReference(blobStorage);

var rotRutServiceDb = sqlServer.AddDatabase("rotRutServiceDb", "RotRutService");
var rotRutService = builder.AddProject<RotRutService>("rotRutService")
.WithReference(rotRutServiceDb)
.WithReference(messaging)
.WithReference(blobStorage);

var documentsDb = sqlServer.AddDatabase("documentsDb", "Documents");
var documents = builder.AddProject<Documents>("documents")
.WithReference(documentsDb)
.WithReference(messaging)
.WithReference(blobStorage);

var messengerDb = sqlServer.AddDatabase("messengerDb", "Messenger");
var messenger = builder.AddProject<Messenger>("messenger")
.WithReference(messengerDb)
.WithReference(messaging)
.WithReference(blobStorage);

var analyticsDb = sqlServer.AddDatabase("analyticsDb", "Analytics");
var analytics = builder.AddProject<Analytics>("analytics")
.WithReference(analyticsDb)
.WithReference(messaging)
.WithReference(blobStorage);

builder.AddYarp("ingress")
       .WithEndpoint(port: 5174, scheme: "https")
       .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
       .WithReference(blobStorage)
       .WithReference(portal)
       .WithReference(appservice)
       .WithReference(identityManagement)
       .WithReference(notifications)
       .WithReference(sales)
       .WithReference(catalog)
       .LoadFromConfiguration("ReverseProxy");

builder.Build().Run();
