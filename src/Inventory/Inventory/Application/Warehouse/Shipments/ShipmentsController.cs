using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Inventory.Application;
using YourBrand.Inventory.Application.Common.Models;
using YourBrand.Inventory.Application.Warehouses.Shipments.Commands;
using YourBrand.Inventory.Application.Warehouses.Shipments.Queries;

using ModelSortDirection = YourBrand.Inventory.Application.Common.Models.SortDirection;

namespace YourBrand.Inventory.Application.Warehouses.Shipments;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/Warehouses/{warehouseId}/Shipments")]
public class ShipmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ItemsResult<ShipmentDto>> GetShipments(
        string warehouseId,
        int page = 1,
        int pageSize = 10,
        string? orderNo = null,
        string? searchString = null,
        string? sortBy = null,
        ModelSortDirection? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetShipments(page - 1, pageSize, warehouseId, orderNo, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{shipmentId}")]
    public async Task<ShipmentDto?> GetShipment(string warehouseId, string shipmentId, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetShipment(warehouseId, shipmentId), cancellationToken);
    }

    [HttpPost]
    public async Task<ShipmentDto> CreateShipment(string warehouseId, CreateShipmentDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateShipment(warehouseId, dto.OrderNo, dto.Destination, dto.Service), cancellationToken);
    }

    [HttpPut("{shipmentId}")]
    public async Task UpdateShipment(string warehouseId, string shipmentId, UpdateShipmentDto dto, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateShipment(warehouseId, shipmentId, dto.Destination, dto.Service), cancellationToken);
    }

    [HttpDelete("{shipmentId}")]
    public async Task DeleteShipment(string warehouseId, string shipmentId, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteShipment(warehouseId, shipmentId), cancellationToken);
    }

    [HttpPost("{shipmentId}/Items")]
    public async Task<ShipmentItemDto> AddShipmentItem(string warehouseId, string shipmentId, AddShipmentItemDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new AddShipmentItem(warehouseId, shipmentId, dto.WarehouseItemId, dto.Quantity), cancellationToken);
    }

    [HttpPut("{shipmentId}/Items/{shipmentItemId}")]
    public async Task<ShipmentItemDto> UpdateShipmentItem(string warehouseId, string shipmentId, string shipmentItemId, UpdateShipmentItemDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new UpdateShipmentItem(warehouseId, shipmentId, shipmentItemId, dto.Quantity), cancellationToken);
    }

    [HttpDelete("{shipmentId}/Items/{shipmentItemId}")]
    public async Task RemoveShipmentItem(string warehouseId, string shipmentId, string shipmentItemId, CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveShipmentItem(warehouseId, shipmentId, shipmentItemId), cancellationToken);
    }

    [HttpPost("{shipmentId}/Ship")]
    public async Task<ShipmentDto> ShipShipment(string warehouseId, string shipmentId, ShipShipmentDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new ShipShipment(warehouseId, shipmentId, dto.ShippedAt), cancellationToken);
    }
}
