using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Inventory.Application.Items.Commands;
using YourBrand.Inventory.Application.Items.Queries;
using YourBrand.Inventory.Application.Warehouses.Items;
using YourBrand.Inventory.Application.Warehouses.Items.Queries;

namespace YourBrand.Inventory.Application.Items;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ItemsResult<ItemDto>> GetItems(int page = 1, int pageSize = 10, string? groupId = null, string? warehouseId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetItems(page - 1, pageSize, groupId, warehouseId, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<ItemDto?> GetItem(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetItem(id), cancellationToken);
    }


    [HttpGet("{id}/Warehouse")]
    public async Task<ItemsResult<WarehouseItemDto>> GetWarehouseItems(string? id = null, int page = 1, int pageSize = 10, string? warehouseId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetWarehouseItems(page - 1, pageSize, warehouseId, id, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpPost]
    public async Task<ItemDto> CreateItem(CreateItemDto dto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new AddItem(dto.Id, dto.Name, dto.Type, dto.GroupId, dto.Unit), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateItem(string id, UpdateItemDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateItem(id, dto.Id, dto.Name, dto.GroupId, dto.Unit), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteItem(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteItem(id), cancellationToken);
    }
}

public record CreateItemDto(string Id, string Name, ItemTypeDto Type, string GroupId, string Unit);

public record UpdateItemDto(string Id, string Name, string GroupId, string Unit);