
using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record ShipWarehouseItems(string WarehouseId, string Id, int Quantity, bool FromPicked = false) : IRequest
{
    public class Handler(IInventoryContext context) : IRequestHandler<ShipWarehouseItems>
    {
        public async Task Handle(ShipWarehouseItems request, CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Quantity));
            }

            var item = await context.WarehouseItems
                .Include(i => i.Reservations)
                .FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Ship(request.Quantity, request.FromPicked);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}