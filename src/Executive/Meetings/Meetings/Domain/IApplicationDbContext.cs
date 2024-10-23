using Microsoft.EntityFrameworkCore;

using YourBrand.Meetings.Domain.Entities;

namespace YourBrand.Meetings.Domain;

public interface IApplicationDbContext
{
    DbSet<Meeting> Meetings { get; }

    DbSet<Agenda> Agendas { get; }

    DbSet<Entities.Minutes> Minutes { get; }

    DbSet<Motion> Motions { get; }

    DbSet<MeetingGroup> MeetingGroups { get; }

    DbSet<User> Users { get; }

    DbSet<Organization> Organizations { get; }

    DbSet<OrganizationUser> OrganizationUsers { get; }

    /*
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }
    */

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}