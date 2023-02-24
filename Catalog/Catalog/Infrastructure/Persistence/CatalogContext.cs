using YourBrand.Catalog.Application.Common.Interfaces;
using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Common;
using YourBrand.Catalog.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Catalog.Infrastructure.Persistence.Interceptors;
using Newtonsoft.Json;
using YourBrand.Catalog.Infrastructure.Persistence.Outbox;

namespace YourBrand.Catalog.Infrastructure.Persistence;

public class CatalogContext : DbContext, ICatalogContext
{
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public CatalogContext(
        DbContextOptions<CatalogContext> options,
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
    }

    public DbSet<ProductGroup> ProductGroups { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<ProductAttribute> ProductAttributes { get; set; } = null!;

    public DbSet<AttributeGroup> AttributeGroups { get; set; } = null!;

    public DbSet<Domain.Entities.Attribute> Attributes { get; set; } = null!;

    public DbSet<AttributeValue> AttributeValues { get; set; } = null!;

    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;

    public DbSet<ProductVariantOption> ProductVariantOptions { get; set; } = null!;

    public DbSet<ProductVariantAttributeValue> ProductVariantAttributeValues { get; set; } = null!;

    public DbSet<ProductOption> ProductOptions { get; set; } = null!;

    public DbSet<OptionGroup> OptionGroups { get; set; } = null!;

    public DbSet<Option> Options { get; set; } = null!;

    public DbSet<OptionValue> OptionValues { get; set; } = null!;

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