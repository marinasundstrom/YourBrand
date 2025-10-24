using System.Linq.Expressions;

using LinqKit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.Entities;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Meetings.Infrastructure.Persistence.ValueConverters;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Infrastructure.Persistence;

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
        configurationBuilder.Properties<AgendaId>().HaveConversion<AgendaIdConverter>();
        configurationBuilder.Properties<AgendaItemId>().HaveConversion<AgendaItemIdConverter>();

        configurationBuilder.Properties<MeetingId>().HaveConversion<MeetingIdConverter>();
        configurationBuilder.Properties<MeetingAttendeeId>().HaveConversion<MeetingAttendeeIdConverter>();
        configurationBuilder.Properties<MeetingAttendeeFunctionId>().HaveConversion<MeetingAttendeeFunctionIdConverter>();

        configurationBuilder.Properties<DebateId>().HaveConversion<DebateIdConverter>();
        configurationBuilder.Properties<DebateEntryId>().HaveConversion<DebateEntryIdConverter>();

        configurationBuilder.Properties<MotionId>().HaveConversion<MotionIdConverter>();
        configurationBuilder.Properties<MotionOperativeClauseId>().HaveConversion<OperativeClauseIdConverter>();

        configurationBuilder.Properties<VotingId>().HaveConversion<VotingIdConverter>();
        configurationBuilder.Properties<VoteId>().HaveConversion<VoteIdConverter>();

        configurationBuilder.Properties<ElectionId>().HaveConversion<ElectionIdConverter>();
        configurationBuilder.Properties<ElectionCandidateId>().HaveConversion<ElectionCandidateIdConverter>();
        configurationBuilder.Properties<BallotId>().HaveConversion<BallotIdConverter>();

        configurationBuilder.Properties<DiscussionId>().HaveConversion<SpeakerIdConverter>();
        configurationBuilder.Properties<SpeakerRequestId>().HaveConversion<SpeakerRequestIdConverter>();

        configurationBuilder.Properties<MinutesId>().HaveConversion<MinutesIdConverter>();
        configurationBuilder.Properties<MinutesAttendeeId>().HaveConversion<MinutesAttendeeIdConverter>();
        configurationBuilder.Properties<MinutesItemId>().HaveConversion<MinutesItemIdConverter>();
        configurationBuilder.Properties<MinutesTaskId>().HaveConversion<MinutesTaskIdConverter>();
        
        configurationBuilder.Properties<MeetingGroupId>().HaveConversion<MeetingGroupIdConverter>();
        configurationBuilder.Properties<MeetingGroupMemberId>().HaveConversion<MeetingGroupMemberIdConverter>();

        configurationBuilder.AddTenantIdConverter();
        configurationBuilder.AddOrganizationIdConverter();
        configurationBuilder.AddUserIdConverter();
    }

#nullable disable

    public DbSet<Meeting> Meetings { get; set; }

    public DbSet<MeetingAttendee> MeetingAttendees { get; set; }

    public DbSet<MeetingAttendeeFunction> MeetingAttendeeFunctions { get; set; }

    public DbSet<AttendeeRole> AttendeeRoles { get; set; }

    public DbSet<MeetingFunction> MeetingFunctions { get; set; }

    public DbSet<Agenda> Agendas { get; set; }

    public DbSet<AgendaItemType> AgendaItemTypes { get; set; }

    public DbSet<Domain.Entities.Minutes> Minutes { get; set; }

    public DbSet<MinutesTask> MinutesTasks { get; set; }

    public DbSet<Motion> Motions { get; set; }

    public DbSet<MeetingGroup> MeetingGroups { get; set; }

    public DbSet<MemberRole> MemberRoles { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<Organization> Organizations { get; set; }

    public DbSet<OrganizationUser> OrganizationUsers { get; set; }

#nullable restore
}