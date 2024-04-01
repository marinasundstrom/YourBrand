
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record UpdateWarehouseItem(string WarehouseId, string Id, string Location) : IRequest
{
    public class Handler : IRequestHandler<UpdateWarehouseItem>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateWarehouseItem request, CancellationToken cancellationToken)
        {
            var item = await _context.WarehouseItems.FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            item.Location = request.Location;

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}