using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Customers.Application.Addresses.Queries;

namespace YourBrand.Customers.Application.Addresses;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class AddressesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ItemsResult<Addresses.AddressDto>>> GetAddresses(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAddresses(page, pageSize), cancellationToken);
        return Ok(result);
    }
}