
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record UpdateWarehouseItem(string WarehouseId, string Id, string Location) : IRequest
{
    public class Handler(IInventoryContext context) : IRequestHandler<UpdateWarehouseItem>
    {
        public async Task Handle(UpdateWarehouseItem request, CancellationToken cancellationToken)
        {
            var item = await context.WarehouseItems.FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Location = request.Location;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}