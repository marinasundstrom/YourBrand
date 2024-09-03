using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain;
using YourBrand.Domain.Common;
using YourBrand.Domain.Entities;
using YourBrand.Identity;
using YourBrand.Infrastructure.Persistence.Interceptors;
using YourBrand.Infrastructure.Persistence.Outbox;
using YourBrand.Tenancy;

namespace YourBrand.Infrastructure.Persistence;

public class AppServiceContext(
    DbContextOptions<AppServiceContext> options, ITenantContext tenantContext) : DbContext(options), IAppServiceContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Configurations.SearchResultItemConfiguration).Assembly);

        ConfigQueryFilterForEntity(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

#nullable disable

    public DbSet<BrandProfile> BrandProfiles { get; set; } = null!;

    public DbSet<SearchResultItem> SearchResultItems { get; set; } = null!;

    public DbSet<Module> Modules { get; set; } = null!;

    public DbSet<TenantModule> TenantModules { get; set; } = null!;

    public DbSet<WidgetArea> WidgetAreas { get; set; } = null!;

    public DbSet<Widget> Widgets { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;

#nullable restore

    private void ConfigQueryFilterForEntity(ModelBuilder modelBuilder)
    {
        foreach (var clrType in modelBuilder.Model
            .GetEntityTypes()
            .Select(entityType => entityType.ClrType))
        {
            if (!clrType.IsAssignableTo(typeof(Entity)))
            {
                continue;
            }

            if (!clrType.Namespace.StartsWith("YourBrand.Domain"))
            {
                continue;
            }

            var entityTypeBuilder = modelBuilder.Entity(clrType);

            if (clrType.IsAssignableTo(typeof(IHasTenant)))
            {
                entityTypeBuilder.HasIndex(nameof(IHasTenant.TenantId));
            }

            if (clrType.IsAssignableTo(typeof(IHasOrganization)))
            {
                entityTypeBuilder.HasIndex(nameof(IHasOrganization.OrganizationId));
            }

            try
            {
                var parameter = Expression.Parameter(clrType, "entity");

                List<Expression> queryFilters = new();

                if (TenancyQueryFilter.CanApplyTo(clrType))
                {
                    var tenantFilter = TenancyQueryFilter.GetFilter(() => tenantContext.TenantId);

                    queryFilters.Add(
                        Expression.Invoke(tenantFilter, Expression.Convert(parameter, typeof(IHasTenant))));
                }

                if (SoftDeleteQueryFilter.CanApplyTo(clrType))
                {
                    var softDeleteFilter = SoftDeleteQueryFilter.GetFilter();

                    queryFilters.Add(
                        Expression.Invoke(softDeleteFilter, Expression.Convert(parameter, typeof(Domain.ISoftDelete))));
                }

                Expression? queryFilter = null;

                foreach (var qf in queryFilters)
                {
                    if (queryFilter is null)
                    {
                        queryFilter = qf;
                    }
                    else
                    {
                        queryFilter = Expression.AndAlso(
                            queryFilter,
                            qf)
                            .Expand();
                    }
                }

                if (queryFilters.Count == 0)
                {
                    continue;
                }

                var queryFilterLambda = Expression.Lambda(queryFilter.Expand(), parameter);

                entityTypeBuilder.HasQueryFilter(queryFilterLambda);
            }
            catch (InvalidOperationException exc)
                when (exc.Message.Contains("cannot be configured as non-owned because it has already been configured as a owned"))
            {
                Console.WriteLine("Skipping previously configured owned type");
            }
            catch (InvalidOperationException exc)
                when (exc.Message.Contains("cannot be added to the model because its CLR type has been configured as a shared type"))
            {
                Console.WriteLine("Skipping previously configured shared type");
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .OrderBy(e => e.Timestamp)
            .ToList();

        var outboxMessages = domainEvents.Select(domainEvent =>
        {
            return new OutboxMessage()
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            };
        }).ToList();

        this.Set<OutboxMessage>().AddRange(outboxMessages);

        return await base.SaveChangesAsync(cancellationToken);
    }
}