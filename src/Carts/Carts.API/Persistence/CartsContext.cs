using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Carts.Domain.Entities;
using YourBrand.Domain;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Carts.Persistence;

public sealed class CartsContext(DbContextOptions options, ITenantContext tenantContext) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>()
            .HasMany(cart => cart.Items)
            .WithOne()
            .HasForeignKey("CartId")
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        modelBuilder.Entity<Cart>().HasIndex(x => x.TenantId);

        modelBuilder.Entity<CartItem>().HasIndex(x => x.TenantId);

        ConfigQueryFilterForEntity(modelBuilder);
    }

    private void ConfigQueryFilterForEntity(ModelBuilder modelBuilder)
    {
        foreach (var clrType in modelBuilder.Model
            .GetEntityTypes()
            .Select(entityType => entityType.ClrType))
        {
            if (!clrType.IsAssignableTo(typeof(Domain.Entities.IAuditable)))
            {
                continue;
            }

            var entityTypeBuilder = modelBuilder.Entity(clrType);

            entityTypeBuilder.AddTenantIndex();

            entityTypeBuilder.AddOrganizationIndex();

            try
            {
                entityTypeBuilder.RegisterQueryFilters(builder =>
                {
                    builder.AddTenancyFilter(tenantContext);
                    builder.AddSoftDeleteFilter();
                });
            }
            catch (InvalidOperationException exc)
                when (exc.MatchQueryFilterExceptions(clrType))
            { }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }


    public DbSet<Cart> Carts { get; set; } = default!;

    public DbSet<CartItem> CartItems { get; set; } = default!;
}