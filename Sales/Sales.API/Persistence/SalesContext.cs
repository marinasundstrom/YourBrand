using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain.Persistence;
using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.API.Persistence;

public sealed class SalesContext : DomainDbContext, IUnitOfWork, ISalesContext
{
    public SalesContext(
        DbContextOptions<SalesContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplySoftDeleteQueryFilter(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesContext).Assembly);
    }

    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        // INFO: This code adds a query filter to any object deriving from Entity
        //       and that is implementing the ISoftDelete interface.
        //       The generated expressions correspond to: (e) => e.Deleted == null.
        //       Causing the entity not to be included in the result if Deleted is not null.
        //       There are other better ways to approach non-destructive "deletion".

        var softDeleteInterface = typeof(ISoftDelete);
        var deletedProperty = softDeleteInterface.GetProperty(nameof(ISoftDelete.Deleted));

        foreach (var entityType in softDeleteInterface.Assembly
            .GetTypes()
            .Where(candidateEntityType => candidateEntityType != typeof(ISoftDelete))
            .Where(candidateEntityType => softDeleteInterface.IsAssignableFrom(candidateEntityType)))
        {
            var param = Expression.Parameter(entityType, "entity");
            var body = Expression.Equal(Expression.Property(param, deletedProperty!), Expression.Constant(null));
            var lambda = Expression.Lambda(body, param);

            modelBuilder.Entity(entityType).HasQueryFilter(lambda);
        }
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