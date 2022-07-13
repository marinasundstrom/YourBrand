using YourBrand.Warehouse.Application;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using YourBrand.Warehouse.Application.Items.Queries;

namespace YourBrand.Warehouse.Controllers;

[Route("[controller]")]
public class ItemsController : ControllerBase 
{
    private readonly IMediator _mediator;

    public ItemsController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<Application.Items.ItemDto>>> GetItems(int page, int pageSize, CancellationToken cancellationToken = default) 
    {
        var result = await _mediator.Send(new GetItems(page, pageSize), cancellationToken);
        return Ok(result);
    }
}
