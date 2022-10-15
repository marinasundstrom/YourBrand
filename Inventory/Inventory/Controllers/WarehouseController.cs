using YourBrand.Inventory.Application;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using YourBrand.Inventory.Application.Warehouses.Queries;
using Microsoft.AspNetCore.Authorization;
using YourBrand.Inventory.Application.Warehouses;
using YourBrand.Inventory.Application.Warehouses.Commands;

namespace YourBrand.Inventory.Controllers;

[Route("Inventory/[controller]")]
[ApiController]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IMediator _mediator;

    public WarehousesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ItemsResult<WarehouseDto>> GetWarehouses(int page = 1, int pageSize = 10, string? siteId = null, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetWarehousesQuery(page - 1, pageSize, siteId, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<WarehouseDto?> GetWarehouse(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetWarehouseQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<WarehouseDto> CreateWarehouse(CreateWarehouseDto dto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateWarehouseCommand(dto.Name, dto.SiteId), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateWarehouse(string id, UpdateWarehouseDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateWarehouseCommand(id, dto.Name, dto.SiteId), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteWarehouse(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteWarehouseCommand(id), cancellationToken);
    }
}

public record CreateWarehouseDto(string Name, string SiteId);

public record UpdateWarehouseDto(string Name, string SiteId);

