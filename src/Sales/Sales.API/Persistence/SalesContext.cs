using System.Linq.Expressions;

using LinqKit;

using MassTransit.Internals;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Domain.Persistence;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Persistence;

public sealed class SalesContext(
    DbContextOptions<SalesContext> options, ITenantContext tenantContext) : DomainDbContext(options), IUnitOfWork, ISalesContext
{
    private readonly string? _tenantId = tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesContext).Assembly);

        ConfigQueryFilterForEntity(modelBuilder);
    }

    private void ConfigQueryFilterForEntity(ModelBuilder modelBuilder)
    {
        foreach (var clrType in modelBuilder.Model
            .GetEntityTypes()
            .Select(entityType => entityType.ClrType))
        {
            if (!clrType.IsAssignableTo(typeof(IHasDomainEvents)))
            {
                Console.WriteLine($"Skipping type {clrType} because it is not implementing IHasDomainEvents.");
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
                when (exc.MatchQueryFilterExceptions(clrType)) {}
        }
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