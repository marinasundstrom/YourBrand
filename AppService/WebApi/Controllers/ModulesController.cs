using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourBrand.Application.Modules;

namespace YourBrand.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
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
}