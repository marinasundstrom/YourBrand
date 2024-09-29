using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

using YourBrand.Ticketing.Infrastructure.Persistence.ValueConverters;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options, ITenantContext tenantContext) : DbContext(options), IUnitOfWork, IApplicationDbContext
{
    private readonly TenantId _tenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
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
                    var softDeleteFilter = SoftDeleteQueryFilter.GetFilter()!;

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

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<TicketId>().HaveConversion<TicketIdConverter>();
        configurationBuilder.Properties<TicketParticipantId>().HaveConversion<TicketParticipantIdConverter>();

        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

#nullable disable

    public DbSet<Ticket> Tickets { get; set; }

    public DbSet<TicketComment> TicketComments { get; set; }

    public DbSet<TicketEvent> TicketEvents { get; set; }

    public DbSet<TicketParticipant> TicketParticipants { get; set; }

    public DbSet<TicketStatus> TicketStatuses { get; set; }

    public DbSet<TicketType> TicketTypes { get; set; }

    public DbSet<TicketCategory> TicketCategories { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Organization> Organizations { get; set; }

    public DbSet<OrganizationUser> OrganizationUsers { get; set; }

#nullable restore
}