﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(p => p.Type);

        builder.HasOne(o => o.Store).WithMany(x => x.Products)
           .HasForeignKey(o => new { o.OrganizationId, o.StoreId });

        builder.HasOne(o => o.Brand).WithMany()
            .HasForeignKey(o => new { o.OrganizationId, o.BrandId })
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Producer).WithMany()
            .HasForeignKey(x => new { x.OrganizationId, x.ProducerId })
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(p => p.Image)
            .WithMany()
            .HasForeignKey(o => new { o.OrganizationId, o.ImageId })
            .IsRequired(false);

        builder
            .Property(x => x.Handle)
            .HasMaxLength(150);

        builder.HasIndex(p => p.OrganizationId);

        builder.HasIndex(p => p.Handle);

        builder.HasMany(x => x.Prices)
            .WithOne(x => x.Product)
            .HasForeignKey(o => new { o.OrganizationId, o.ProductId })
            .OnDelete(DeleteBehavior.ClientCascade);

        builder
            .Ignore(b => b.Price);

        builder
            .Ignore(b => b.RegularPrice);
        
        builder
            .Ignore(b => b.VatRate);
        
        builder
            .Ignore(b => b.VatRateId);

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
            .HasMany(x => x.Variants)
            .WithOne(x => x.Parent)
            .HasForeignKey(x => new { x.OrganizationId, x.ParentId })
            .IsRequired(false)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(x => x.ProductOptions)
            .WithOne(x => x.Product)
            .HasForeignKey(x => new { x.OrganizationId, x.ProductId })
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.ProductVariantOptions)
            .WithOne(x => x.ProductVariant)
            .HasForeignKey(x => new { x.OrganizationId, x.ProductVariantId })
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(p => p.Category)
            .WithMany(p => p.Products)
            .HasForeignKey(x => new { x.OrganizationId, x.CategoryId })
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasMany(x => x.SubscriptionPlans)
            .WithOne(x => x.Product)
            .HasForeignKey(x => new { x.OrganizationId, x.ProductId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}