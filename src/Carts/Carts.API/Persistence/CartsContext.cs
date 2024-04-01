using Microsoft.EntityFrameworkCore;

using YourBrand.Carts.API.Domain.Entities;

namespace YourBrand.Carts.API.Persistence;

public sealed class CartsContext : DbContext
{
    public CartsContext(DbContextOptions options) : base(options)
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

    public DbSet<Cart> Carts { get; set; } = default!;

    public DbSet<CartItem> CartItems { get; set; } = default!;
}