using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Shipments.Commands;

public record AddShipmentItem(string WarehouseId, string ShipmentId, string WarehouseItemId, int Quantity) : IRequest<ShipmentItemDto>;

public class AddShipmentItemHandler(IInventoryContext context) : IRequestHandler<AddShipmentItem, ShipmentItemDto>
{
    public async Task<ShipmentItemDto> Handle(AddShipmentItem request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Quantity));
        }

        var shipment = await context.Shipments
            .Include(x => x.Items)
                .ThenInclude(x => x.WarehouseItem)
                    .ThenInclude(x => x.Item)
                        .ThenInclude(x => x.Group)
            .Include(x => x.Items)
                .ThenInclude(x => x.WarehouseItem)
                    .ThenInclude(x => x.Warehouse)
                        .ThenInclude(x => x.Site)
            .FirstOrDefaultAsync(x => x.Id == request.ShipmentId && x.WarehouseId == request.WarehouseId, cancellationToken)
            ?? throw new InvalidOperationException("Shipment not found.");

        var warehouseItem = await context.WarehouseItems
            .Include(x => x.Item)
                .ThenInclude(x => x.Group)
            .Include(x => x.Warehouse)
                .ThenInclude(x => x.Site)
            .FirstOrDefaultAsync(x => x.Id == request.WarehouseItemId && x.WarehouseId == request.WarehouseId, cancellationToken)
            ?? throw new InvalidOperationException("Warehouse item not found.");

        var shipmentItem = shipment.AddItem(warehouseItem, request.Quantity);

        await context.SaveChangesAsync(cancellationToken);

        var persisted = await context.ShipmentItems
            .Include(x => x.WarehouseItem)
                .ThenInclude(x => x.Item)
                    .ThenInclude(x => x.Group)
            .Include(x => x.WarehouseItem)
                .ThenInclude(x => x.Warehouse)
                    .ThenInclude(x => x.Site)
            .AsNoTracking()
            .FirstAsync(x => x.Id == shipmentItem.Id, cancellationToken);

        return persisted.ToDto();
    }
}
