using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence;

public sealed class CatalogContext : DbContext
{
    public CatalogContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);
    }

    public DbSet<Store> Stores { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = default!;

    public DbSet<ProductCategory> ProductCategories { get; set; } = default!;

    public DbSet<ProductImage> ProductImages { get; set; } = default!;

    public DbSet<Brand> Brands { get; set; } = null!;

    public DbSet<ProductCategoryAttribute> ProductCategoryAttributes { get; set; } = null!;

    public DbSet<ProductAttribute> ProductAttributes { get; set; } = null!;

    public DbSet<AttributeGroup> AttributeGroups { get; set; } = null!;

    public DbSet<Domain.Entities.Attribute> Attributes { get; set; } = null!;

    public DbSet<AttributeValue> AttributeValues { get; set; } = null!;

    public DbSet<ProductOption> ProductOptions { get; set; } = null!;

    public DbSet<ProductVariantOption> ProductVariantOptions { get; set; } = null!;

    public DbSet<OptionGroup> OptionGroups { get; set; } = null!;

    public DbSet<Option> Options { get; set; } = null!;

    public DbSet<SelectableOption> SelectableOptions { get; set; } = null!;

    public DbSet<ChoiceOption> ChoiceOptions { get; set; } = null!;

    public DbSet<NumericalValueOption> NumericalValueOptions { get; set; } = null!;

    public DbSet<TextValueOption> TextValueOptions { get; set; } = null!;

    public DbSet<OptionValue> OptionValues { get; set; } = null!;

    public DbSet<Currency> Currencies { get; set; } = null!;

    public DbSet<VatRate> VatRates { get; set; } = null!;
}