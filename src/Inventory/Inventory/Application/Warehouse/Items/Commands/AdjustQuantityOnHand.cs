
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record AdjustQuantityOnHand(string WarehouseId, string Id, int NewQuantityOnHand) : IRequest
{
    public class Handler(IInventoryContext context) : IRequestHandler<AdjustQuantityOnHand>
    {
        public async Task Handle(AdjustQuantityOnHand request, CancellationToken cancellationToken)
        {
            var item = await context.WarehouseItems.FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.AdjustQuantityOnHand(request.NewQuantityOnHand);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}