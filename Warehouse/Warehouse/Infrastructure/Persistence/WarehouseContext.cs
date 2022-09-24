using YourBrand.Warehouse.Application.Common.Interfaces;
using YourBrand.Warehouse.Domain;
using YourBrand.Warehouse.Domain.Common;
using YourBrand.Warehouse.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Warehouse.Infrastructure.Persistence.Interceptors;
using YourBrand.Warehouse.Infrastructure.Persistence.Outbox;
using Newtonsoft.Json;

namespace YourBrand.Warehouse.Infrastructure.Persistence;

public class WarehouseContext : DbContext, IWarehouseContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public WarehouseContext(
        DbContextOptions<WarehouseContext> options,
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

        modelBuilder.HasSequence<int>("WarehouseIds");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseContext).Assembly);
    }

    public DbSet<Item> Items { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
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