using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Domain;

public interface IApplicationDbContext
{
    DbSet<Meeting> Meetings { get; }

    DbSet<MeetingAttendee> MeetingAttendees { get; }

    DbSet<MeetingAttendeeFunction> MeetingAttendeeFunctions { get; }

    DbSet<AttendeeRole> AttendeeRoles { get; }

    DbSet<MeetingFunction> MeetingFunctions { get; }

    DbSet<Agenda> Agendas { get; }

    DbSet<AgendaItemType> AgendaItemTypes { get; }

    DbSet<Entities.Minutes> Minutes { get; }

    DbSet<MinutesTask> MinutesTasks { get; }

    DbSet<Motion> Motions { get; }

    DbSet<MeetingGroup> MeetingGroups { get; }

    DbSet<MemberRole> MemberRoles { get; }

    DbSet<User> Users { get; }

    DbSet<Organization> Organizations { get; }

    DbSet<OrganizationUser> OrganizationUsers { get; }

    /*
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }
    */

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}