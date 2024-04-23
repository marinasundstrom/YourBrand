
using System.Linq.Expressions;

using LinqKit;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Domain.Common;
using YourBrand.IdentityManagement.Domain.Entities;
using YourBrand.IdentityManagement.Infrastructure.Persistence.Configurations;
using YourBrand.IdentityManagement.Infrastructure.Persistence.Outbox;
using YourBrand.Tenancy;

namespace YourBrand.IdentityManagement.Infrastructure.Persistence;

public class ApplicationDbContextWithTenantFilter(
    DbContextOptions<ApplicationDbContextWithTenantFilter> options,
    ITenantContext tenantContext) : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>(options), IApplicationDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);

        ConfigQueryFilterForEntity(modelBuilder);
    }

    private void ConfigQueryFilterForEntity(ModelBuilder modelBuilder)
    {
        foreach (var clrType in modelBuilder.Model
            .GetEntityTypes()
            .Select(entityType => entityType.ClrType))
        {
            /*
            if (!clrType.IsAssignableTo(typeof(Entity)))
            {
                continue;
            }
            */

            try
            {
                var entityTypeBuilder = modelBuilder.Entity(clrType);

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
                        Expression.Invoke(softDeleteFilter, Expression.Convert(parameter, typeof(ISoftDelete))));
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

    public DbSet<Tenant> Tenants { get; set; } = default!;

    public DbSet<Organization> Organizations { get; set; } = default!;

    public DbSet<OrganizationUser> OrganizationUsers { get; set; } = default!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
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
                Id = domainEvent.Id,
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

        var r = await base.SaveChangesAsync(cancellationToken);

        entities.ToList().ForEach(x => x.ClearDomainEvents());

        return r;
    }
}