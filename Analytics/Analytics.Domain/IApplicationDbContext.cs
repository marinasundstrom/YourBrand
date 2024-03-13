using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using YourBrand.Analytics.Domain.Entities;

namespace YourBrand.Analytics.Domain;

public interface IApplicationDbContext
{
    DbSet<Client> Clients { get; }

    DbSet<Session> Sessions { get; }

    DbSet<Event> Events { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}