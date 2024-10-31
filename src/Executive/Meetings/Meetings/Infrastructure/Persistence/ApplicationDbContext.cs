using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Meetings.Infrastructure.Persistence.ValueConverters;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Infrastructure.Persistence;

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
                when (exc.MatchQueryFilterExceptions(clrType))
            { }
        }
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<AgendaId>().HaveConversion<AgendaIdConverter>();
        configurationBuilder.Properties<AgendaItemId>().HaveConversion<AgendaItemIdConverter>();
        configurationBuilder.Properties<MeetingId>().HaveConversion<MeetingIdConverter>();
        configurationBuilder.Properties<MeetingAttendeeId>().HaveConversion<MeetingAttendeeIdConverter>();
        configurationBuilder.Properties<DebateId>().HaveConversion<DebateIdConverter>();
        configurationBuilder.Properties<DebateEntryId>().HaveConversion<DebateEntryIdConverter>();
        configurationBuilder.Properties<MotionId>().HaveConversion<MotionIdConverter>();
        configurationBuilder.Properties<MotionOperativeClauseId>().HaveConversion<OperativeClauseIdConverter>();
        configurationBuilder.Properties<VotingSessionId>().HaveConversion<VotingSessionIdConverter>();
        configurationBuilder.Properties<ElectionCandidateId>().HaveConversion<ElectionCandidateIdConverter>();
        configurationBuilder.Properties<VoteId>().HaveConversion<VoteIdConverter>();
        configurationBuilder.Properties<SpeakerSessionId>().HaveConversion<SpeakerSessionIdConverter>();
        configurationBuilder.Properties<SpeakerRequestId>().HaveConversion<SpeakerRequestIdConverter>();
        configurationBuilder.Properties<MinutesId>().HaveConversion<MinutesIdConverter>();
        configurationBuilder.Properties<MinutesAttendeeId>().HaveConversion<MinutesAttendeeIdConverter>();
        configurationBuilder.Properties<MinutesItemId>().HaveConversion<MinutesItemIdConverter>();
        configurationBuilder.Properties<MeetingGroupId>().HaveConversion<MeetingGroupIdConverter>();
        configurationBuilder.Properties<MeetingGroupMemberId>().HaveConversion<MeetingGroupMemberIdConverter>();

        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

#nullable disable

    public DbSet<Meeting> Meetings { get; set; }

    public DbSet<Agenda> Agendas { get; set; }

    public DbSet<Domain.Entities.Minutes> Minutes { get; set; }

    public DbSet<Motion> Motions { get; set; }

    public DbSet<MeetingGroup> MeetingGroups { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Organization> Organizations { get; set; }

    public DbSet<OrganizationUser> OrganizationUsers { get; set; }

#nullable restore
}