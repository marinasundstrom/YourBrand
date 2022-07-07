using YourBrand.Catalog.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace YourBrand.Catalog.Domain;

public interface ICatalogContext
{
    DbSet<ProductGroup> ProductGroups { get; } 

    DbSet<Product> Products { get; }

    DbSet<ProductAttribute> ProductAttributes { get; }

    DbSet<AttributeGroup> AttributeGroups { get; }

    DbSet<Entities.Attribute> Attributes { get; } 

    DbSet<AttributeValue> AttributeValues { get; }

    DbSet<ProductVariant> ProductVariants { get; }

    DbSet<VariantValue> VariantValues { get; }

    DbSet<ProductOption> ProductOptions { get; }

    DbSet<OptionGroup> OptionGroups { get; }

    DbSet<Option> Options { get; }

    DbSet<OptionValue> OptionValues { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry(object entity);
}