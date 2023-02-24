using YourBrand.Orders.Application.Common.Interfaces;
using YourBrand.Orders.Domain;
using YourBrand.Orders.Domain.Common;
using YourBrand.Orders.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Orders.Infrastructure.Persistence.Interceptors;
using Newtonsoft.Json;
using YourBrand.Orders.Infrastructure.Persistence.Outbox;

namespace YourBrand.Orders.Infrastructure.Persistence;

public class OrdersContext : DbContext, IOrdersContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public OrdersContext(
        DbContextOptions<OrdersContext> options,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging(); 
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersContext).Assembly);
    }

#nullable disable

public DbSet<Order> Orders { get; set; }
public DbSet<OrderStatus> OrderStatuses { get; set; }
public DbSet<OrderTotals> OrderTotals { get; set; }
public DbSet<OrderItem> OrderItems { get; set; }
public DbSet<OrderCharge> OrderCharges { get; set; }
public DbSet<OrderDiscount> OrderDiscounts { get; set; }

public DbSet<Subscription> Subscriptions { get; set; }
public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

public DbSet<PriceList> PriceLists { get; set; }
public DbSet<PriceListItem> PriceListItems { get; set; }

public DbSet<CustomField> CustomFields { get; set; }
public DbSet<CustomFieldDefinition> CustomFieldDefinitions { get; set; }

#nullable restore

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .OrderBy(e => e.Timestamp)
            .ToList();

        var outboxMessages = domainEvents.Select(domainEvent =>
        {
            return new OutboxMessage()
            {
                Id = domainEvent.Id,
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