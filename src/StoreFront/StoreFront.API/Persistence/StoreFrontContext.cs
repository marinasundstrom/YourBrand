using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.StoreFront.API.Domain.Entities;
using YourBrand.Tenancy;

namespace YourBrand.StoreFront.API.Persistence;

public sealed class StoreFrontContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>()
            .HasMany(cart => cart.Items)
            .WithOne()
            .HasForeignKey("CartId")
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

    public DbSet<Cart> StoreFront { get; set; } = default!;

    public DbSet<CartItem> CartItems { get; set; } = default!;
}