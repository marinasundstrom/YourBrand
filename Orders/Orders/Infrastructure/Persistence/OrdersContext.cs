using YourBrand.Orders.Application.Common.Interfaces;
using YourBrand.Orders.Domain;
using YourBrand.Orders.Domain.Common;
using YourBrand.Orders.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Orders.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Orders.Infrastructure.Persistence;

public class OrdersContext : DbContext, IOrdersContext
{
    private IDomainEventService _domainEventService;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public OrdersContext(
        DbContextOptions<OrdersContext> options,
        IDomainEventService domainEventService,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _domainEventService = domainEventService;
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
        await DispatchEvents();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchEvents()
    {
        var entities = ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _domainEventService.Publish(domainEvent);
    }
}