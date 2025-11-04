using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Shipments.Commands;

public record ShipShipment(string WarehouseId, string ShipmentId, DateTimeOffset? ShippedAt = null) : IRequest<ShipmentDto>;

public class ShipShipmentHandler(IInventoryContext context) : IRequestHandler<ShipShipment, ShipmentDto>
{
    public async Task<ShipmentDto> Handle(ShipShipment request, CancellationToken cancellationToken)
    {
        var shipment = await context.Shipments
            .Include(x => x.Warehouse)
                .ThenInclude(x => x.Site)
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

        shipment.Ship(request.ShippedAt);

        await context.SaveChangesAsync(cancellationToken);

        return shipment.ToDto();
    }
}
