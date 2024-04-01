using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record DeleteWarehouseItem(string WarehouseId, string Id) : IRequest
{
    public class Handler : IRequestHandler<DeleteWarehouseItem>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteWarehouseItem request, CancellationToken cancellationToken)
        {
            var item = await _context.WarehouseItems.FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.Id, cancellationToken);

            if (item is null) throw new Exception();

            _context.WarehouseItems.Remove(item);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}