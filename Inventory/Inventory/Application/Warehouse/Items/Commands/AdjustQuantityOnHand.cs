
using YourBrand.Inventory.Domain;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record AdjustQuantityOnHand(string WarehouseId, string Id, int NewQuantityOnHand) : IRequest
{
    public class Handler : IRequestHandler<AdjustQuantityOnHand>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task Handle(AdjustQuantityOnHand request, CancellationToken cancellationToken)
        {
            var item = await _context.WarehouseItems.FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.AdjustQuantityOnHand(request.NewQuantityOnHand);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
