using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Application.Themes;

namespace YourBrand.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class ThemesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ThemeDto>>> GetThemes(
    [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var modules = await mediator.Send(new GetThemesQuery(), cancellationToken);
        return Ok(modules);
    }

    [HttpGet("Current")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[AllowAnonymous]
    public async Task<ActionResult<ThemeDto>> GetTheme(
        [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var modules = await mediator.Send(new GetThemeQuery(), cancellationToken);
        return Ok(modules);
    }

    [HttpPost("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[AllowAnonymous]
    public async Task<ActionResult<ThemeDto>> UpdateTheme(
        string id,
        UpdateThemeRequest request,
        [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var modules = await mediator.Send(new UpdateTheme(id, request.Name, request.Description, request.ColorSchemes), cancellationToken);
        return Ok(modules);
    }

    [HttpPost("{id}/Copy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[AllowAnonymous]
    public async Task<ActionResult<ThemeDto>> CopyTheme(
        string id,
        [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var modules = await mediator.Send(new CopyTheme(id), cancellationToken);
        return Ok(modules);
    }
}

public record UpdateThemeRequest(string Name, string? Description, ThemeColorSchemesDto ColorSchemes);