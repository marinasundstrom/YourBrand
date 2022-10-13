using YourBrand.Warehouse.Application;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using YourBrand.Warehouse.Application.Items.Queries;
using Microsoft.AspNetCore.Authorization;
using YourBrand.Warehouse.Application.Items;
using YourBrand.Warehouse.Application.Items.Commands;

namespace YourBrand.Warehouse.Controllers;

[Route("Warehouse/[controller]")]
[ApiController]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ItemsResult<ItemDto>> GetItems(int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetItems(page - 1, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<ItemDto?> GetItem(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetItem(id), cancellationToken);
    }

    [HttpPost]
    public async Task<ItemDto> CreateItem(CreateItemDto dto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateItem(dto.Name,  dto.SKU), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task UpdateItem(string id, UpdateItemDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateItem(id, dto.Name, dto.SKU), cancellationToken);
    }

    [HttpPut("{id}/QuantityOnHand")]
    public async Task AdjustQuantityOnHand(string id, AdjustQuantityOnHandDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new AdjustQuantityOnHand(id, dto.Quantity), cancellationToken);
    }

    [HttpPut("{id}/Reserve")]
    public async Task ReserveItems(string id, ReserveItemsDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ReserveItems(id, dto.Quantity), cancellationToken);
    }

    [HttpPut("{id}/Pick")]
    public async Task PickItems(string id, PickItemsDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new PickItems(id, dto.Quantity, dto.FromReserved), cancellationToken);
    }

    [HttpPut("{id}/Ship")]
    public async Task ShipItems(string id, ShipItemsDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ShipItems(id, dto.Quantity, dto.FromPicked), cancellationToken);
    }

    [HttpPut("{id}/Receive")]
    public async Task ReceiveItems(string id, ReceiveItemsDto dto, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ReceiveItems(id, dto.Quantity), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteItem(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteItem(id), cancellationToken);
    }
}

public record AdjustQuantityOnHandDto(int Quantity);

public record ReserveItemsDto(int Quantity);

public record PickItemsDto(int Quantity, bool FromReserved = false);

public record ShipItemsDto(int Quantity, bool FromPicked = false);

public record ReceiveItemsDto(int Quantity);

public record CreateItemDto(string Name, string SKU);

public record UpdateItemDto(string Name, string SKU);

