using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record AddWarehouseItem(string ItemId, string WarehouseId, string Location, int QuantityOnHand, int QuantityThreshold) : IRequest<WarehouseItemDto>
{
    public class Handler(IInventoryContext context) : IRequestHandler<AddWarehouseItem, WarehouseItemDto>
    {
        public async Task<WarehouseItemDto> Handle(AddWarehouseItem request, CancellationToken cancellationToken)
        {
            var existing = await context.WarehouseItems
                .FirstOrDefaultAsync(i => i.WarehouseId == request.WarehouseId && i.ItemId == request.ItemId, cancellationToken);

            if (existing is not null)
            {
                throw new InvalidOperationException("The warehouse already tracks this item.");
            }

            var warehouse = await context.Warehouses
                .Include(w => w.Site)
                .FirstOrDefaultAsync(w => w.Id == request.WarehouseId, cancellationToken)
                ?? throw new InvalidOperationException("Warehouse not found.");

            var item = await context.Items
                .Include(i => i.Group)
                .FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken)
                ?? throw new InvalidOperationException("Item not found.");

            var warehouseItem = new Domain.Entities.WarehouseItem(item, warehouse, request.Location, request.QuantityOnHand, request.QuantityThreshold);

            context.WarehouseItems.Add(warehouseItem);

            await context.SaveChangesAsync(cancellationToken);

            var persisted = await context.WarehouseItems
               .Include(x => x.Item)
               .ThenInclude(x => x.Group)
               .Include(x => x.Warehouse)
               .ThenInclude(x => x.Site)
               .AsNoTracking()
               .FirstAsync(c => c.Id == warehouseItem.Id, cancellationToken);

            return persisted.ToDto();
        }
    }
}