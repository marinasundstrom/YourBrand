using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Marketing.Application.Addresses.Queries;

namespace YourBrand.Marketing.Application.Addresses;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class AddressesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ItemsResult<AddressDto>>> GetAddresses(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetAddresses(page, pageSize), cancellationToken);
        return Ok(result);
    }
}