# Domain services

This outlines how to add the domain services to your service:

Has not been implemented by all services.

Reference: Sales service

Projects: 
* Domain
* Domain.Infrastructure
* Domain.Persistence
* Domain.Auditability*
* Identity*
* SoftDelete*
* Tenancy*

## Domain objects

Some guidelines for implementing entity types.

Entities should implement ``IEntity`` or derived interfaces, such as ``IAuditableEntity`` (for basic auditability).

Entities that emit domain events implement ``IHasDomainEvents``. This makes it so that the background job picks them up.

A User is represented by a unique ``UserId``. Maps to a string in Database.

Entities that belong to a particular tenant implement ``IHasTenant``. This works with ``ITenantContext``, and the ``TenantId`` is automatically set when the entities are persisted. And objects are automatically filtered out when queries are run, based on the user's tenant.

You can of course create base classes that implement these.

## Tenancy and Identity

A core concept is Tenant, and it has Users. It may also have Organizations to which Users belong, but that we are saving for another time.

Both TenantId and UserId are passed as claims in the JWT.

This adds ``ITenantContext`` and ``IUserContext``:

```csharp
builder.Services.AddHttpContextAccessor();

builder.Services
    .AddUserContext()
    .AddTenantContext();
```

## DbContext

```csharp
using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Domain.Outbox;
using YourBrand.Tenancy;

namespace YourBrand.MyService.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DomainDbContext
{
    private readonly ITenantContext tenantContext;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ITenantContext tenantContext) : base(options)
    {
        this.tenantContext = tenantContext;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        ConfigQueryFilterForEntity(modelBuilder);
    }

    private void ConfigQueryFilterForEntity(ModelBuilder modelBuilder)
    {
        foreach (var clrType in modelBuilder.Model
            .GetEntityTypes()
            .Select(entityType => entityType.ClrType))
        {
            // Customize this for the particular service

            if (!clrType.IsAssignableTo(typeof(IEntity)))
            {
                Console.WriteLine($"Skipping type {clrType} because it is not implementing IEntity.");
                continue;
            }

            var entityTypeBuilder = modelBuilder.Entity(clrType);


            // Add indexes

            entityTypeBuilder.AddTenantIndex();

            entityTypeBuilder.AddOrganizationIndex();


            // Create query filter

            try
            {
                entityTypeBuilder.RegisterQueryFilters(builder =>
                {
                    builder.AddTenancyFilter(tenantContext);
                    builder.AddSoftDeleteFilter();
                });
            }
            catch (InvalidOperationException exc)
            when (exc.MatchQueryFilterExceptions(clrType))
            { }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Converters for Ids

        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();

        configurationBuilder.Properties<ItemId>().HaveConversion<ItemIdConverter>();
    }

#nullable disable

    public DbSet<Item> Items { get; set; }

#nullable restore

}
```

### Register DbContext

Project: Domain.Infrastructure

```csharp
using Microsoft.EntityFrameworkCore;

using YourBrand.Auditability;
using YourBrand.Domain.Persistence;

namespace YourBrand.MyService.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("db");

        // Required for Transactional Outbox pattern to work (DbContext must inherit DomainDbContext)
        services.AddDomainPersistence<SalesContext>(configuration);

        services.AddDbContext<SalesContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());


            // Use interceptors

            options
                .UseDomainInterceptors(serviceProvider)
                .UseTenancyInterceptor(serviceProvider)
                .UseAuditabilityInterceptor(serviceProvider);
        });

        // Add interceptors

        services.AddTenancyInterceptor();
        services.AddAuditabilityInterceptor();

        return services;
    }
}
```

### Add infrastructure

Project: Domain.Infrastructure

This will add services used for the transactional outbox pattern, such as the background job.

```csharp
service.AddDomainInfrastructure(builder.Configuration);
```

## Asynchronous messaging

This lets TenantId and UserId being passed to other services together with asynchronous messages. 

On the receiving end, the ``ITenantContext`` and ``IUserContext`` services will be automatically initialized.


```csharp
    public static IServiceCollection AddMassTransit(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumers(typeof(Program).Assembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.UseTenancyFilters(context); // THIS
                cfg.UseIdentityFilters(context); // THIS

                cfg.ConfigureEndpoints(context); // THIS
            });
        });

        return services;
    }
```

## SignalR

In the case of SignalR, Tenancy and Identity is not transiently passed. You need to set it manually using the ``ISettableTenantContext`` and ``ISettableUserContext`` services.

Example also shows how to persist data about a connection.

```csharp
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

using YourBrand.Tenancy;

namespace YourBrand.MyService.Hubs;

public record MessageData(string ChannelId, string Content);

public interface IChatHub
{
    Task<string> PostMessage(string channelId, string content);
}

public interface IChatHubClient
{
    Task OnMessagePosted(MessageData message);
}

public record ConnectionState(string TenantId, string OrganizationId, string ChannelId);

[Authorize]
public sealed class TestHub : Hub<ITestHubClient>, ITestHub
{
    private readonly IMediator mediator;
    private readonly ISettableUserContext userContext;
    private readonly ISettableTenantContext tenantContext;
    private readonly static Dictionary<string, ConnectionState> state = new Dictionary<string, ConnectionState>();

    public TestHub(IMediator mediator, ISettableUserContext userContext, ISettableTenantContext tenantContext)
    {
        this.mediator = mediator;
        this.userContext = userContext;
        this.tenantContext = tenantContext;
    }

    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        if (httpContext is not null)
        {
            var tenantId = httpContext?.User?.FindFirst("tenant_id")?.Value;

            if (httpContext.Request.Query.TryGetValue("organizationId", out var organizationId))
            {

            }

            if (httpContext.Request.Query.TryGetValue("channelId", out var channelId))
            {
                Groups.AddToGroupAsync(this.Context.ConnectionId, $"channel-{channelId}");
            }

            state.Add(Context.ConnectionId, new ConnectionState(tenantId, organizationId, channelId));
        }

        return base.OnConnectedAsync();
    }

    public async Task<string> PostMessage(string channelId, string? replyTo, string content)
    {
        var connectionState = state[Context.ConnectionId];

        // Setting the contexts manually

        tenantContext.SetTenantId(connectionState.TenantId);
        userContext.SetCurrentUser(Context.User!);
        userContext.SetConnectionId(Context.ConnectionId);

        return (string)await mediator.Send(
            new PostMessage(s.OrganizationId, channelId, replyTo, content));
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        state.Remove(Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }
}
```

## Event handlers

Also event handler execute outside of a tenant context:

```csharp
public record Test(TenantId TenantId, UserId UserId, string Other) : DomainEvent; 

public sealed class TestEventHandler(ISettableTenantContext tenantContext, ISettableUserContext userContext, ILogger<TestEventHandler> logger) : IDomainEventHandler<Test>
{
    public async Task Handle(Test notification, CancellationToken cancellationToken)
    {
tenantContext.SetTenantId(notification.TenantId);
userContext.SetCurrentUser(notification.User!);
}
}
```
