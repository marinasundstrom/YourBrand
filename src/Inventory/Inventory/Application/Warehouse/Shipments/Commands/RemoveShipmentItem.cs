using System;
using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Shipments.Commands;

public record RemoveShipmentItem(string WarehouseId, string ShipmentId, string ShipmentItemId) : IRequest;

public class RemoveShipmentItemHandler(IInventoryContext context) : IRequestHandler<RemoveShipmentItem>
{
    public async Task Handle(RemoveShipmentItem request, CancellationToken cancellationToken)
    {
        var shipment = await context.Shipments
            .Include(x => x.Items)
                .ThenInclude(x => x.WarehouseItem)
            .FirstOrDefaultAsync(x => x.Id == request.ShipmentId && x.WarehouseId == request.WarehouseId, cancellationToken)
            ?? throw new InvalidOperationException("Shipment not found.");

        var shipmentItem = shipment.Items.FirstOrDefault(x => x.Id == request.ShipmentItemId)
            ?? throw new InvalidOperationException("Shipment item not found.");

        shipment.RemoveItem(shipmentItem.Id);

        context.ShipmentItems.Remove(shipmentItem);

        await context.SaveChangesAsync(cancellationToken);
    }
}
