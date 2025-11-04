using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Shipments.Queries;

public record GetShipment(string WarehouseId, string ShipmentId) : IRequest<ShipmentDto?>;

public class GetShipmentHandler(IInventoryContext context) : IRequestHandler<GetShipment, ShipmentDto?>
{
    public async Task<ShipmentDto?> Handle(GetShipment request, CancellationToken cancellationToken)
    {
        var shipment = await context.Shipments
            .AsNoTracking()
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
            .FirstOrDefaultAsync(x => x.Id == request.ShipmentId && x.WarehouseId == request.WarehouseId, cancellationToken);

        return shipment?.ToDto();
    }
}
