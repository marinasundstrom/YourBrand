
using YourBrand.Inventory.Domain;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record ReserveWarehouseItems2(string WarehouseId, string Id, int Quantity) : IRequest
{
    public class Handler : IRequestHandler<ReserveWarehouseItems2>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task Handle(ReserveWarehouseItems2 request, CancellationToken cancellationToken)
        {
            var item = await _context.WarehouseItems.FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Reserve(request.Quantity);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
