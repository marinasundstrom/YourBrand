using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Enums;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record ReleaseWarehouseItemReservation(string WarehouseId, string Id, string ReservationId) : IRequest
{
    public class Handler(IInventoryContext context) : IRequestHandler<ReleaseWarehouseItemReservation>
    {
        public async Task Handle(ReleaseWarehouseItemReservation request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.ReservationId))
            {
                throw new ArgumentNullException(nameof(request.ReservationId));
            }

            var item = await context.WarehouseItems
                .Include(i => i.Reservations)
                .FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null)
            {
                throw new Exception();
            }

            item.ReleaseReservation(request.ReservationId, WarehouseItemReservationStatus.Released);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
