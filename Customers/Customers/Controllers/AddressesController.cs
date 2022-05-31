using YourBrand.Customers.Application;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using YourBrand.Customers.Application.Addresses.Queries;

namespace YourBrand.Customers.Controllers;

[Route("[controller]")]
public class AddressesController : ControllerBase 
{
    private readonly IMediator _mediator;

    public AddressesController(IMediator mediator) 
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<Customers.Application.Addresses.AddressDto>>> GetAddresses(int page, int pageSize, CancellationToken cancellationToken = default) 
    {
        var result = await _mediator.Send(new GetAddresses(page, pageSize), cancellationToken);
        return Ok(result);
    }
}