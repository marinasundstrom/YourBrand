using YourBrand.Products.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace YourBrand.Products.Domain;

public interface IProductsContext
{
    DbSet<ProductGroup> ProductGroups { get; } 

    DbSet<Product> Products { get; }

    DbSet<ProductOption> ProductOptions { get; }

    DbSet<OptionGroup> OptionGroups { get; }

    DbSet<Option> Options { get; } 

    DbSet<OptionValue> OptionValues { get; }

    DbSet<ProductVariant> ProductVariants { get; }

    DbSet<VariantValue> VariantValues { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry(object entity);
}