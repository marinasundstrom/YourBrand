using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourBrand.Application.Modules;

namespace YourBrand.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[AllowAnonymous]
public class ModulesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModules(
        [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        var modules = await mediator.Send(new GetModulesQuery(), cancellationToken);
        return Ok(modules);
    }

    [HttpPost("{id}/Toggle")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ToggleModule(Guid id,
    [FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new ToggleModule(id), cancellationToken);
        return Ok();
    }
}