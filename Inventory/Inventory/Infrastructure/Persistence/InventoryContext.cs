using YourBrand.Inventory.Application.Common.Interfaces;
using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Inventory.Infrastructure.Persistence.Interceptors;
using YourBrand.Inventory.Infrastructure.Persistence.Outbox;
using Newtonsoft.Json;

namespace YourBrand.Inventory.Infrastructure.Persistence;

public class InventoryContext : DbContext, IInventoryContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public InventoryContext(
        DbContextOptions<InventoryContext> options,
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

        modelBuilder.HasSequence<int>("InventoryIds");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryContext).Assembly);
    }

    public DbSet<Site> Sites { get; set; } = null!;
    public DbSet<Warehouse> Warehouses { get; set; } = null!;
    public DbSet<WarehouseItem> WarehouseItems { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<ItemGroup> ItemGroups { get; set; } = null!;

    public DbSet<Item> Items { get; set; } = null!;

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