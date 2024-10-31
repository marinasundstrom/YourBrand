# Domain services

This outlines how to add the domain services to your service:

Reference: Sales service

Projects: 
* Domain
* Domain.Infrastructure
* Domain.Persistence
* Domain.Auditability*
* Identity*
* SoftDelete*
* Tenancy*

## Tenancy and Identity

This adds ``ITenantContext`` and ``IUserContext``:

```csharp
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
            // Customize this for the partivular service

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