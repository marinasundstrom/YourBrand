using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Common;
using YourBrand.Customers.Domain.Entities;
using YourBrand.Customers.Infrastructure.Persistence.Interceptors;
using YourBrand.Customers.Infrastructure.Persistence.Outbox;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Customers.Infrastructure.Persistence;

public class CustomersContext(
    DbContextOptions<CustomersContext> options,
    ITenantContext tenantContext) : DbContext(options), ICustomersContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasSequence<int>("CustomerIds");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CustomersContext).Assembly);

        ConfigQueryFilterForEntity(modelBuilder);
    }

    private void ConfigQueryFilterForEntity(ModelBuilder modelBuilder)
    {
        foreach (var clrType in modelBuilder.Model
            .GetEntityTypes()
            .Select(entityType => entityType.ClrType))
        {
            if (!clrType.IsAssignableTo(typeof(IHasDomainEvents)))
            {
                continue;
            }

            if (!clrType.IsAbstract)
            {
                if (!clrType.BaseType.Name.StartsWith("Entity") && !clrType.BaseType.Name.StartsWith("AggregateRoot"))
                {
                    Console.WriteLine($"Skipping entity {clrType} because it is not a base type: " + clrType.BaseType.Name);
                    continue;
                }
            }

            var entityTypeBuilder = modelBuilder.Entity(clrType);

            entityTypeBuilder.AddTenantIndex();

            entityTypeBuilder.AddOrganizationIndex();

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
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

    public DbSet<Customer> Customers { get; set; } = null!;

    public DbSet<Person> Persons { get; set; } = null!;

    public DbSet<Organization> Organizations { get; set; } = null!;

    public DbSet<Address> Addresses { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        if (!entities.Any())
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        var domainEvents = entities
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents.ToList();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .OrderBy(e => e.Timestamp)
            .ToList();

        var outboxMessages = domainEvents.Select(domainEvent =>
        {
            return new OutboxMessage()
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            };
        }).ToList();

        this.Set<OutboxMessage>().AddRange(outboxMessages);

        return await base.SaveChangesAsync(cancellationToken);
    }
}