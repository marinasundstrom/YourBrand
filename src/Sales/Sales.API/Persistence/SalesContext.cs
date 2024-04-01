using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain.Persistence;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Persistence;

public sealed class SalesContext : DomainDbContext, IUnitOfWork, ISalesContext
{
    private readonly string? _tenantId;

    public SalesContext(
        DbContextOptions<SalesContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantId = tenantService.TenantId;
    }

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
            try
            {
                var entityTypeBuilder = modelBuilder.Entity(clrType);

                var parameter = Expression.Parameter(clrType, "entity");
                Expression? queryFilter = null;

                var softDeleteFilter = ApplySoftDeleteQueryFilter(clrType);
                if (softDeleteFilter is not null)
                {
                    queryFilter = Expression.Invoke(softDeleteFilter, Expression.Convert(parameter, typeof(ISoftDelete)));
                }

                var tenantFilter = ConfigTenantFilter(clrType);
                if (tenantFilter is not null)
                {
                    if (queryFilter is null)
                    {
                        queryFilter = Expression.Invoke(tenantFilter, Expression.Convert(parameter, typeof(IHasTenant)));
                    }
                    else
                    {
                        queryFilter = Expression.AndAlso(
                            queryFilter,
                             Expression.Invoke(tenantFilter, Expression.Convert(parameter, typeof(IHasTenant))))
                            .Expand();
                    }
                }

                if (queryFilter is null)
                {
                    continue;
                }

                var queryFilterLambda = Expression.Lambda(queryFilter.Expand(), parameter);

                entityTypeBuilder.HasQueryFilter(queryFilterLambda);
            }
            catch (InvalidOperationException exc)
                when (exc.Message.Contains("cannot be configured as non-owned because it has already been configured as a owned"))
            {
                Console.WriteLine("Skipping owned type");
            }
            catch (InvalidOperationException exc)
                when (exc.Message.Contains("cannot be added to the model because its CLR type has been configured as a shared type"))
            {
                Console.WriteLine("Skipping shared type");
            }
        }
    }

    private Expression<Func<IHasTenant, bool>>? ConfigTenantFilter(Type entityType)
    {
        var hasTenantInterface = typeof(IHasTenant);
        if (!hasTenantInterface.IsAssignableFrom(entityType))
        {
            return null;
        }
        return (IHasTenant e) => e.TenantId == _tenantId;
    }

    private Expression<Func<ISoftDelete, bool>>? ApplySoftDeleteQueryFilter(Type entityType)
    {
        var softDeleteInterface = typeof(ISoftDelete);
        var deletedProperty = softDeleteInterface.GetProperty(nameof(ISoftDelete.Deleted));

        if (!softDeleteInterface.IsAssignableFrom(entityType))
        {
            return null;
        }

        var param = Expression.Parameter(softDeleteInterface, "entity");
        var body = Expression.Equal(Expression.Property(param, deletedProperty!), Expression.Constant(null));
        return Expression.Lambda<Func<ISoftDelete, bool>>(body, param);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<TenantId>().HaveConversion<TenantIdConverter>();
    }

#nullable disable

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<OrderStatus> OrderStatuses { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Subscription> Subscriptions { get; set; }

    public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

#nullable restore
}