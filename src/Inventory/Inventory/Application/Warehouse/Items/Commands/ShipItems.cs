
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
            var item = await context.WarehouseItems.FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Ship(request.Quantity, request.FromPicked);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}