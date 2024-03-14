using YourBrand.Catalog.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder
            .Property(x => x.Handle)
            .HasMaxLength(150);

        builder.HasIndex(p => p.Handle);

        builder
            .HasOne(p => p.Image)
            .WithMany()
            .HasForeignKey(x => x.ImageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull); ;

        builder
            .HasMany(p => p.Images)
            .WithOne(x => x.Product)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .HasMany(p => p.Options)
            .WithMany(p => p.Products)
            .UsingEntity<ProductOption>();

        builder
            .HasMany(x => x.OptionGroups)
            .WithOne(x => x.Product)
            .OnDelete(DeleteBehavior.ClientNoAction);

        /*
        builder
            .HasMany(p => p.Attributes)
            .WithMany(p => p.Products)
            .UsingEntity<ProductAttribute>();
        */

        builder
            .HasOne(x => x.Parent)
            .WithMany(x => x.Variants)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(x => x.ProductOptions)
            .WithOne(x => x.Product)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(p => p.Category)
            .WithMany(p => p.Products);
    }
}
