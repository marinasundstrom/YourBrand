using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Application.BrandProfiles;

namespace YourBrand.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class BrandProfileController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[AllowAnonymous]
    public async Task<ActionResult<BrandProfileDto>> GetBrandProfile(
        [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var modules = await mediator.Send(new GetBrandProfileQuery(), cancellationToken);
        return Ok(modules);
    }
}