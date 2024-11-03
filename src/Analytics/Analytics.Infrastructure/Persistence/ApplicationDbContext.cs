using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Analytics.Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ApplySoftDeleteQueryFilter(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        // INFO: This code adds a query filter to any object deriving from Entity
        //       and that is implementing the ISoftDeletable interface.
        //       The generated expressions correspond to: (e) => e.Deleted == n ull.
        //       Causing the entity not to be included in the result if Deleted is not null.
        //       There are other better ways to approach non-destructive "deletion".

        var softDeleteInterface = typeof(ISoftDeletable);
        var deletedProperty = softDeleteInterface.GetProperty(nameof(ISoftDeletable.IsDeleted));

        foreach (var entityType in softDeleteInterface.Assembly
            .GetTypes()
            .Where(x => !x.IsInterface)
            .Where(candidateEntityType => candidateEntityType != typeof(ISoftDeletable))
            .Where(candidateEntityType => softDeleteInterface.IsAssignableFrom(candidateEntityType)))
        {
            try 
            {
                var param = Expression.Parameter(entityType, "entity");
                var body = Expression.Equal(Expression.Property(param, deletedProperty!), Expression.Constant(false));
                var lambda = Expression.Lambda(body, param);

                modelBuilder.Entity(entityType).HasQueryFilter(lambda);

                Console.WriteLine($"Applied soft-delete filter: {entityType}");
            }
            catch (Exception exc) 
            {
                Console.WriteLine($"Failed to apply soft-delete filter: {entityType}");
            }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

#nullable disable

    public DbSet<Client> Clients { get; set; } = null!;

    public DbSet<Session> Sessions { get; set; } = null!;

    public DbSet<Event> Events { get; set; } = null!;

#nullable restore
}