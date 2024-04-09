using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record DeleteWarehouseItem(string WarehouseId, string Id) : IRequest
{
    public class Handler(IInventoryContext context) : IRequestHandler<DeleteWarehouseItem>
    {
        public async Task Handle(DeleteWarehouseItem request, CancellationToken cancellationToken)
        {
            var item = await context.WarehouseItems.FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            context.WarehouseItems.Remove(item);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}