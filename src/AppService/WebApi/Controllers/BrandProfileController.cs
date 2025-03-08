using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Application.BrandProfiles;
using YourBrand.Application.Themes;

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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[AllowAnonymous]
    public async Task<ActionResult<BrandProfileDto>> UpdateBrandProfile(
        UpdateBrandProfileRequest request,
        [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var modules = await mediator.Send(new UpdateBrandProfile(request.Name, request.Description, request.Theme), cancellationToken);
        return Ok(modules);
    }
}

public record UpdateBrandProfileRequest(string Name, string? Description, ThemeDto Theme);