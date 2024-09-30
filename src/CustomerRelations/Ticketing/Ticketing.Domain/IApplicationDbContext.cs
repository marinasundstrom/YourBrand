using Microsoft.EntityFrameworkCore;

using YourBrand.Ticketing.Domain.Entities;

namespace YourBrand.Ticketing.Domain;

public interface IApplicationDbContext
{
    DbSet<Ticket> Tickets { get; }

    DbSet<TicketComment> TicketComments { get; }

    DbSet<TicketEvent> TicketEvents { get; }

    DbSet<TicketParticipant> TicketParticipants { get; }

    DbSet<TicketCategory> TicketCategories { get; }

    DbSet<TicketStatus> TicketStatuses { get; }

    DbSet<Project> Projects { get; }

    DbSet<ProjectGroup> ProjectGroups { get; }

    DbSet<ProjectMembership> ProjectMemberships { get; }

    DbSet<Team> Teams { get; }

    DbSet<TeamMembership> TeamMemberships { get; }

    DbSet<User> Users { get; }

    DbSet<Organization> Organizations { get; }

    DbSet<OrganizationUser> OrganizationUsers { get; }


    /*
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    public DbSet<OutboxMessageConsumer> OutboxMessageConsumers { get; set; }
    */

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}