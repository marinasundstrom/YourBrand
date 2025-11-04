using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Shipments.Commands;

public record UpdateShipmentItem(string WarehouseId, string ShipmentId, string ShipmentItemId, int Quantity) : IRequest<ShipmentItemDto>;

public class UpdateShipmentItemHandler(IInventoryContext context) : IRequestHandler<UpdateShipmentItem, ShipmentItemDto>
{
    public async Task<ShipmentItemDto> Handle(UpdateShipmentItem request, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.Quantity));
        }

        var shipmentItem = await context.ShipmentItems
            .Include(x => x.Shipment)
            .Include(x => x.WarehouseItem)
                .ThenInclude(x => x.Item)
                    .ThenInclude(x => x.Group)
            .Include(x => x.WarehouseItem)
                .ThenInclude(x => x.Warehouse)
                    .ThenInclude(x => x.Site)
            .FirstOrDefaultAsync(x => x.Id == request.ShipmentItemId && x.ShipmentId == request.ShipmentId, cancellationToken)
            ?? throw new InvalidOperationException("Shipment item not found.");

        if (shipmentItem.Shipment.WarehouseId != request.WarehouseId)
        {
            throw new InvalidOperationException("Shipment item does not belong to the specified warehouse.");
        }

        shipmentItem.ChangeQuantity(request.Quantity);

        await context.SaveChangesAsync(cancellationToken);

        return shipmentItem.ToDto();
    }
}
