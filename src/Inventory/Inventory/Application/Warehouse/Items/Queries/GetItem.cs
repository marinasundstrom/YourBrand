using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Queries;

public record GetWarehouseItem(string WarehouseId, string Id) : IRequest<WarehouseItemDto?>
{
    public class Handler(IInventoryContext context) : IRequestHandler<GetWarehouseItem, WarehouseItemDto?>
    {
        public async Task<WarehouseItemDto?> Handle(GetWarehouseItem request, CancellationToken cancellationToken)
        {
            var person = await context.WarehouseItems
                .Include(x => x.Item)
                .ThenInclude(x => x.Group)
                .Include(x => x.Warehouse)
                .ThenInclude(x => x.Site)
                .Include(x => x.Reservations)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.WarehouseId == request.WarehouseId && x.ItemId == request.Id, cancellationToken);

            return person?.ToDto();
        }
    }
}