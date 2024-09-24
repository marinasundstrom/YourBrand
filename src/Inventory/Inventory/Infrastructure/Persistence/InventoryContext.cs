using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.Entities;
using YourBrand.Inventory.Infrastructure.Persistence.Interceptors;
using YourBrand.Inventory.Infrastructure.Persistence.Outbox;
using YourBrand.Tenancy;

namespace YourBrand.Inventory.Infrastructure.Persistence;

public class InventoryContext(
    DbContextOptions<InventoryContext> options) : DbContext(options), IInventoryContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasSequence<int>("InventoryIds");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
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