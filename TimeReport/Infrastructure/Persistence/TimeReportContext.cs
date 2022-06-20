
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Services;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Infrastructure.Persistence.Configurations;

namespace YourBrand.TimeReport.Infrastructure.Persistence;

public class TimeReportContext : DbContext, ITimeReportContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ITenantService _tenantService;
    private readonly IDomainEventService _domainEventService;
    private readonly IDateTime _dateTime;
    private readonly string _organizationId;

    public TimeReportContext(
        DbContextOptions<TimeReportContext> options,
        ICurrentUserService currentUserService,
        ITenantService tenantService,
        IDomainEventService domainEventService,
        IDateTime dateTime,
        IApiApplicationContext apiApplicationContext) : base(options)
    {
        _currentUserService = currentUserService;
        _tenantService = tenantService;
        _domainEventService = domainEventService;
        _dateTime = dateTime;

        _organizationId = _tenantService.OrganizationId!;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging(); //.LogTo(Console.WriteLine);
#endif

        base.OnConfiguring(optionsBuilder);
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
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<AuditableEntity> entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    if(entry.Entity is IHasTenant e)
                    {
                        e.OrganizationId = _tenantService.OrganizationId!;
                    }

                    entry.Entity.CreatedById = _currentUserService.UserId;
                    entry.Entity.Created = _dateTime.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedById = _currentUserService.UserId;
                    entry.Entity.LastModified = _dateTime.Now;
                    break;

                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.DeletedById = _currentUserService.UserId;
                        softDelete.Deleted = _dateTime.Now;

                        entry.State = EntityState.Modified;
                    }
                    break;
            }
        }

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