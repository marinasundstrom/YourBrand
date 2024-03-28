
using YourBrand.Inventory.Domain;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record ShipWarehouseItems(string WarehouseId, string Id, int Quantity, bool FromPicked = false) : IRequest
{
    public class Handler : IRequestHandler<ShipWarehouseItems>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task Handle(ShipWarehouseItems request, CancellationToken cancellationToken)
        {
            var item = await _context.WarehouseItems.FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Ship(request.Quantity, request.FromPicked);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
