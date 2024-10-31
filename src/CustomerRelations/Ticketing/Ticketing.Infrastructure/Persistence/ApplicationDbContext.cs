using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;
using YourBrand.Ticketing.Infrastructure.Persistence.ValueConverters;

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
        configurationBuilder.Properties<TicketId>().HaveConversion<TicketIdConverter>();
        configurationBuilder.Properties<TicketParticipantId>().HaveConversion<TicketParticipantIdConverter>();
        configurationBuilder.Properties<ProjectId>().HaveConversion<ProjectIdConverter>();

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

    public DbSet<Project> Projects { get; set; }

    public DbSet<ProjectGroup> ProjectGroups { get; set; }

    public DbSet<ProjectMembership> ProjectMemberships { get; set; }

    public DbSet<Team> Teams { get; set; }

    public DbSet<TeamMembership> TeamMemberships { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Organization> Organizations { get; set; }

    public DbSet<OrganizationUser> OrganizationUsers { get; set; }

#nullable restore
}