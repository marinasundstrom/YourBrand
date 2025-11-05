using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Inventory.Application.Common.Models;
using YourBrand.Inventory.Application.Warehouses.TransferOrders.Commands;
using YourBrand.Inventory.Application.Warehouses.TransferOrders.Queries;

using ModelSortDirection = YourBrand.Inventory.Application.Common.Models.SortDirection;

namespace YourBrand.Inventory.Application.Warehouses.TransferOrders;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class TransferOrdersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ItemsResult<TransferOrderDto>> GetTransferOrders(
        int page = 1,
        int pageSize = 10,
        string? sourceWarehouseId = null,
        string? destinationWarehouseId = null,
        string? searchString = null,
        string? sortBy = null,
        ModelSortDirection? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetTransferOrders(page - 1, pageSize, sourceWarehouseId, destinationWarehouseId, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<TransferOrderDto?> GetTransferOrder(string id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetTransferOrder(id), cancellationToken);
    }

    [HttpPost]
    public async Task<TransferOrderDto> CreateTransferOrder(CreateTransferOrderDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateTransferOrder(dto.SourceWarehouseId, dto.DestinationWarehouseId, dto.Lines), cancellationToken);
    }

    [HttpPost("{id}/Complete")]
    public async Task<TransferOrderDto> CompleteTransferOrder(string id, CompleteTransferOrderDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CompleteTransferOrder(id, dto.CompletedAt), cancellationToken);
    }
}
