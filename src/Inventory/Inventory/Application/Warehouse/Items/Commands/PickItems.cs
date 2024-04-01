
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record PickWarehouseItems(string WarehouseId, string Id, int Quantity, bool FromReserved = false) : IRequest
{
    public class Handler : IRequestHandler<PickWarehouseItems>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task Handle(PickWarehouseItems request, CancellationToken cancellationToken)
        {
            var item = await _context.WarehouseItems.FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Pick(request.Quantity, request.FromReserved);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}