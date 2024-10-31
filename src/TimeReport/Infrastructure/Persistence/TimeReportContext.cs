
using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

using YourBrand.ApiKeys;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Infrastructure.Persistence.Configurations;
using YourBrand.TimeReport.Infrastructure.Persistence.Interceptors;
using YourBrand.TimeReport.Infrastructure.Persistence.Outbox;

namespace YourBrand.TimeReport.Infrastructure.Persistence;

public class TimeReportContext : DbContext, ITimeReportContext
{
    private readonly ITenantContext _tenantContext;
    private readonly string _tenantId;

    public TimeReportContext(
        DbContextOptions<TimeReportContext> options,
        ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
        _tenantId = _tenantContext.TenantId!;
    }

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
            if (!clrType.IsAssignableTo(typeof(Entity)))
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
                    builder.AddTenancyFilter(_tenantContext);
                    builder.AddSoftDeleteFilter();
                });
            }
            catch (InvalidOperationException exc)
                when (exc.MatchQueryFilterExceptions(clrType))
            { }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
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

    public DbSet<ProjectTeam> ProjectTeams { get; set; } = null!;

    public DbSet<Expense> Expenses { get; set; } = null!;

    public DbSet<ExpenseType> ExpenseTypes { get; set; } = null!;

    public DbSet<Activity> Activities { get; set; } = null!;

    public DbSet<ActivityType> ActivityTypes { get; set; } = null!;

    public DbSet<Entry> Entries { get; set; } = null!;

    public DbSet<TimeSheet> TimeSheets { get; set; } = null!;

    public DbSet<ReportingPeriod> ReportingPeriods { get; set; } = null!;

    public DbSet<TimeSheetActivity> TimeSheetActivities { get; set; } = null!;

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entities = ChangeTracker
                        .Entries<Entity>()
                        .Where(e => e.Entity.DomainEvents.Any())
                        .Select(e => e.Entity);

        if (!entities.Any())
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        var domainEvents = entities
            .SelectMany(entity =>
            {
                var domainEvents = entity.DomainEvents.ToList();

                entity.ClearDomainEvents();

                return domainEvents;
            })
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

        return await base.SaveChangesAsync(cancellationToken);
    }
}