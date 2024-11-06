using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Domain.Persistence;
using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Persistence;

public sealed class SalesContext(
    DbContextOptions<SalesContext> options, ITenantContext tenantContext) : DbContext(options), IUnitOfWork, ISalesContext
{
    public TenantId? TenantId => tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .ApplyDomainEntityConfigurations()
            .ApplyConfigurationsFromAssembly(typeof(SalesContext).Assembly);

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

#nullable disable

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<OrderType> OrderTypes { get; set; }

    public DbSet<OrderStatus> OrderStatuses { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Subscription> Subscriptions { get; set; }

    public DbSet<SubscriptionType> SubscriptionTypes { get; set; }

    public DbSet<SubscriptionStatus> SubscriptionStatuses { get; set; }

    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

#nullable restore
}