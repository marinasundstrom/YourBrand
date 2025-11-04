using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Shipments.Commands;

public record UpdateShipment(string WarehouseId, string ShipmentId, string Destination, string Service) : IRequest;

public class UpdateShipmentHandler(IInventoryContext context) : IRequestHandler<UpdateShipment>
{
    public async Task Handle(UpdateShipment request, CancellationToken cancellationToken)
    {
        var shipment = await context.Shipments
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x => x.Id == request.ShipmentId && x.WarehouseId == request.WarehouseId, cancellationToken)
            ?? throw new InvalidOperationException("Shipment not found.");

        shipment.ChangeDestination(request.Destination);
        shipment.ChangeService(request.Service);

        await context.SaveChangesAsync(cancellationToken);
    }
}
