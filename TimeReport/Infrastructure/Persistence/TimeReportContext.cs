
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using YourBrand.ApiKeys;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Infrastructure.Persistence.Configurations;
using YourBrand.TimeReport.Infrastructure.Persistence.Interceptors;

namespace YourBrand.TimeReport.Infrastructure.Persistence;

public class TimeReportContext : DbContext, ITimeReportContext
{
    private readonly ITenantService _tenantService;
    private readonly IDomainEventService _domainEventService;
    private readonly AuditableEntitySaveChangesInterceptor _auditableEntitySaveChangesInterceptor;
    private readonly string _organizationId;

    public TimeReportContext(
        DbContextOptions<TimeReportContext> options,
        ITenantService tenantService,
        IDomainEventService domainEventService,
        IApiApplicationContext apiApplicationContext,
        AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor) : base(options)
    {
        _tenantService = tenantService;
        _domainEventService = domainEventService;
        _auditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
        _organizationId = _tenantService.OrganizationId!;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(_auditableEntitySaveChangesInterceptor);

#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging(); 
#endif
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);

        // Tenant filters

        modelBuilder.Entity<Team>().HasQueryFilter(x => x.OrganizationId == _organizationId);

        modelBuilder.Entity<Project>().HasQueryFilter(x => x.OrganizationId == _organizationId);
    }

    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Organization> Organizations { get; set; } = null!;

    public DbSet<OrganizationUser> OrganizationUsers { get; set; } = null!;

    public DbSet<Team> Teams { get; set; } = null!;

    public DbSet<TeamMembership> TeamMemberships { get; set; } = null!;

    public DbSet<Absence> Absence { get; set; } = null!;

    public DbSet<AbsenceType> AbsenceTypes { get; set; } = null!;

    public DbSet<Project> Projects { get; set; } = null!;

    public DbSet<ProjectGroup> ProjectGroups { get; set; } = null!;

    public DbSet<ProjectMembership> ProjectMemberships { get; set; } = null!;

    public DbSet<ProjectTeam> ProjectTeams{ get; set; } = null!;

    public DbSet<Expense> Expenses { get; set; } = null!;

    public DbSet<ExpenseType> ExpenseTypes { get; set; } = null!;

    public DbSet<Activity> Activities { get; set; } = null!;

    public DbSet<ActivityType> ActivityTypes { get; set; } = null!;

    public DbSet<Entry> Entries { get; set; } = null!;

    public DbSet<TimeSheet> TimeSheets { get; set; } = null!;

    public DbSet<MonthEntryGroup> TimeSheetMonths { get; set; } = null!;

    public DbSet<TimeSheetActivity> TimeSheetActivities { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await DispatchEvents();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchEvents()
    {
        var entities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity);

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ToList().ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
            await _domainEventService.Publish(domainEvent);
    }
}