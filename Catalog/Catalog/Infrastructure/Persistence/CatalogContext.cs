using YourBrand.Catalog.Application.Common.Interfaces;
using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Common;
using YourBrand.Catalog.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Catalog.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Catalog.Infrastructure.Persistence;

public class CatalogContext : DbContext, ICatalogContext
{
    private readonly IDomainEventService _domainEventService;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;

    public CatalogContext(
        DbContextOptions<CatalogContext> options,
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

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
    }

    public DbSet<ProductGroup> ProductGroups { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = null!;

    public DbSet<ProductAttribute> ProductAttributes { get; set; } = null!;

    public DbSet<AttributeGroup> AttributeGroups { get; set; } = null!;

    public DbSet<Domain.Entities.Attribute> Attributes { get; set; } = null!;

    public DbSet<AttributeValue> AttributeValues { get; set; } = null!;

    public DbSet<ProductVariant> ProductVariants { get; set; } = null!;

    public DbSet<VariantValue> VariantValues { get; set; } = null!;

    public DbSet<ProductOption> ProductOptions { get; set; } = null!;

    public DbSet<OptionGroup> OptionGroups { get; set; } = null!;

    public DbSet<Option> Options { get; set; } = null!;

    public DbSet<OptionValue> OptionValues { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchEvents();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchEvents()
    {
        var entities = ChangeTracker
            .Entries<BaseEntity>()
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