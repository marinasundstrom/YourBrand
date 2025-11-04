
using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record PickWarehouseItems(string WarehouseId, string Id, int Quantity, bool FromReserved = false) : IRequest
{
    public class Handler(IInventoryContext context) : IRequestHandler<PickWarehouseItems>
    {
        public async Task Handle(PickWarehouseItems request, CancellationToken cancellationToken)
        {
            if (request.Quantity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(request.Quantity));
            }

            var item = await context.WarehouseItems.FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Pick(request.Quantity, request.FromReserved);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}