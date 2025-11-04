using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Shipments.Commands;

public record DeleteShipment(string WarehouseId, string ShipmentId) : IRequest;

public class DeleteShipmentHandler(IInventoryContext context) : IRequestHandler<DeleteShipment>
{
    public async Task Handle(DeleteShipment request, CancellationToken cancellationToken)
    {
        var shipment = await context.Shipments
            .Include(x => x.Items)
                .ThenInclude(x => x.WarehouseItem)
            .FirstOrDefaultAsync(x => x.Id == request.ShipmentId && x.WarehouseId == request.WarehouseId, cancellationToken)
            ?? throw new InvalidOperationException("Shipment not found.");

        shipment.Cancel();

        context.Shipments.Remove(shipment);

        await context.SaveChangesAsync(cancellationToken);
    }
}
