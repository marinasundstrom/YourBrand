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
    public TenantId? TenantId => tenantContext.TenantId;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.ConfigureDomainModel(configurator =>
        {
            configurator.AddTenancyFilter(() => TenantId);
            configurator.AddSoftDeleteFilter();
        });
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