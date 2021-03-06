
using YourBrand.Messenger;
using YourBrand.Messenger.Application;
using YourBrand.Messenger.Authentication;
using YourBrand.Messenger.Hubs;
using YourBrand.Messenger.Infrastructure;
using YourBrand.Messenger.Infrastructure.Persistence;

using MassTransit;

using NSwag;
using NSwag.Generation.Processors.Security;

var builder = WebApplication.CreateBuilder(args);

if (args.Contains("--connection-string"))
{
    builder.Configuration["ConnectionStrings:DefaultConnection"] = args[args.ToList().IndexOf("--connection-string") + 1];
}

var Configuration = builder.Configuration;

var services = builder.Services;

services.AddApplication(Configuration);
services.AddInfrastructure(Configuration);
services.AddServices();

services
    .AddControllers()
    .AddNewtonsoftJson();

services.AddSignalR();

builder.Services.AddHttpContextAccessor();

services.AddEndpointsApiExplorer();

// Register the Swagger services
services.AddOpenApiDocument(document =>
{
    document.Title = "Messenger API";
    document.Version = "v1";

    document.AddSecurity("JWT", new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: Bearer {your JWT token}."
    });

    document.AddSecurity("ApiKey", new OpenApiSecurityScheme
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = OpenApiSecurityApiKeyLocation.Header,
        Description = "Type into the textbox: {your API key}."
    });

    document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
    document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("ApiKey"));
});

services.AddAuthWithJwt();
services.AddAuthWithApiKey();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumers(typeof(Program).Assembly);
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseOpenApi();
    app.UseSwaggerUi3(c =>
    {
        c.DocumentTitle = "Messenger API v1";
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.MapHub<MessageHub>("/hubs/messages");

var configuration = app.Services.GetService<IConfiguration>();

if (args.Contains("--seed"))
{
    Console.WriteLine("Seeding");

    await app.Services.SeedAsync();
    return;
}

app.Run();