using Microsoft.EntityFrameworkCore;

using YourBrand.Notifications.Domain.Entities;

namespace YourBrand.Notifications.Application.Common.Interfaces;

public interface IWorkerContext
{
    DbSet<Notification> Notifications { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}