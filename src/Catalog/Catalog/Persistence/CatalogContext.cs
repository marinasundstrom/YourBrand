using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Persistence;

public sealed class CatalogContext(
    DbContextOptions<CatalogContext> options,
    ITenantContext tenantContext,
    ILogger<CatalogContext> logger) : DbContext(options)
{
    public TenantId? TenantId => tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogContext).Assembly);

        modelBuilder.ConfigureDomainModel(configurator =>
        {
            configurator.AddTenancyFilter(() => TenantId);
            configurator.AddSoftDeleteFilter();
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

    public DbSet<Store> Stores { get; set; } = null!;

    public DbSet<Product> Products { get; set; } = default!;

    public DbSet<ProductSubscriptionPlan> ProductSubscriptionPlan { get; set; } = default!;

    public DbSet<ProductCategory> ProductCategories { get; set; } = default!;

    public DbSet<ProductImage> ProductImages { get; set; } = default!;

    public DbSet<Brand> Brands { get; set; } = null!;

    public DbSet<Producer> Producers { get; set; } = null!;

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