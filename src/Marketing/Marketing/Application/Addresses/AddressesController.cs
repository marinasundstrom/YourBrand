using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Marketing.Application.Addresses.Queries;

namespace YourBrand.Marketing.Application.Addresses;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class AddressesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AddressesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ItemsResult<AddressDto>>> GetAddresses(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAddresses(page, pageSize), cancellationToken);
        return Ok(result);
    }
}