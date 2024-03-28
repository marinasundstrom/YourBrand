using Microsoft.EntityFrameworkCore;

using YourBrand.StoreFront.API.Domain.Entities;

namespace YourBrand.StoreFront.API.Persistence;

public sealed class StoreFrontContext : DbContext
{
    public StoreFrontContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>()
            .HasMany(cart => cart.Items)
            .WithOne()
            .HasForeignKey("CartId")
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);
    }

    public DbSet<Cart> StoreFront { get; set; } = default!;

    public DbSet<CartItem> CartItems { get; set; } = default!;
}