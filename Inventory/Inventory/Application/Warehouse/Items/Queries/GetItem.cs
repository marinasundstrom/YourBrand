using YourBrand.Inventory.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Inventory.Application.Warehouses.Items;

namespace YourBrand.Inventory.Application.Warehouses.Items.Queries;

public record GetWarehouseItem(string WarehouseItemId) : IRequest<WarehouseItemDto?>
{
    public class Handler : IRequestHandler<GetWarehouseItem, WarehouseItemDto?>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task<WarehouseItemDto?> Handle(GetWarehouseItem request, CancellationToken cancellationToken)
        {
            var person = await _context.WarehouseItems
                .Include(x => x.Item)
                .ThenInclude(x => x.Group)
                .Include(x => x.Warehouse)
                .ThenInclude(x => x.Site)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.WarehouseItemId, cancellationToken);

            return person?.ToDto();
        }
    }
}