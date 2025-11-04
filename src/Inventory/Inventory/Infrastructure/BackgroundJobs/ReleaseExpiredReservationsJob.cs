using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Quartz;

using YourBrand.Inventory.Domain.Enums;
using YourBrand.Inventory.Infrastructure.Persistence;

namespace YourBrand.Inventory.Infrastructure.BackgroundJobs;

public class ReleaseExpiredReservationsJob : IJob
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReleaseExpiredReservationsJob> _logger;

    public ReleaseExpiredReservationsJob(IServiceProvider serviceProvider, ILogger<ReleaseExpiredReservationsJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryContext>();

        var now = DateTimeOffset.UtcNow;

        var reservations = await dbContext.WarehouseItemReservations
            .Include(r => r.WarehouseItem)
            .ThenInclude(i => i.Reservations)
            .Where(r => r.Status == WarehouseItemReservationStatus.Pending && r.ExpiresAt <= now)
            .ToListAsync(context.CancellationToken);

        if (reservations.Count == 0)
        {
            return;
        }

        foreach (var reservation in reservations)
        {
            try
            {
                reservation.WarehouseItem.ReleaseReservation(reservation.Id, WarehouseItemReservationStatus.Expired);
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "Failed to release expired reservation {ReservationId}", reservation.Id);
            }
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
