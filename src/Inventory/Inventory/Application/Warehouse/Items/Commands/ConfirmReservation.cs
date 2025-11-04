using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record ConfirmWarehouseItemReservation(string WarehouseId, string Id, string ReservationId) : IRequest
{
    public class Handler(IInventoryContext context) : IRequestHandler<ConfirmWarehouseItemReservation>
    {
        public async Task Handle(ConfirmWarehouseItemReservation request, CancellationToken cancellationToken)
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

            item.ConfirmReservation(request.ReservationId);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
