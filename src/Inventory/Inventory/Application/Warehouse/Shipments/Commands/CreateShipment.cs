using System;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Application.Warehouses.Shipments.Commands;

public record CreateShipment(string WarehouseId, string OrderNo, string Destination, string Service) : IRequest<ShipmentDto>;

public class CreateShipmentHandler(IInventoryContext context) : IRequestHandler<CreateShipment, ShipmentDto>
{
    public async Task<ShipmentDto> Handle(CreateShipment request, CancellationToken cancellationToken)
    {
        var warehouse = await context.Warehouses
            .Include(x => x.Site)
            .FirstOrDefaultAsync(x => x.Id == request.WarehouseId, cancellationToken)
            ?? throw new InvalidOperationException("Warehouse not found.");

        var shipment = new Shipment(warehouse, request.OrderNo, request.Destination, request.Service);

        context.Shipments.Add(shipment);

        await context.SaveChangesAsync(cancellationToken);

        var persisted = await context.Shipments
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
            .AsNoTracking()
            .FirstAsync(x => x.Id == shipment.Id, cancellationToken);

        return persisted.ToDto();
    }
}
