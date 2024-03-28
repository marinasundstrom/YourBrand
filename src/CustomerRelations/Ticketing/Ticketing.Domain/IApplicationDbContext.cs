using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using YourBrand.Ticketing.Domain.Entities;

namespace YourBrand.Ticketing.Domain;

public interface IApplicationDbContext
{
    DbSet<TicketStatus> TicketStatuses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}