using Asp.Versioning;

using IdentityModel.Client;

using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Application.Modules;
using YourBrand.Domain.Entities;

namespace YourBrand.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class ModulesController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [AllowAnonymous]
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
        //var foo = await HttpContext.GetTokenAsync("access_token");
        //Console.WriteLine(foo);

        await mediator.Send(new ToggleModule(id), cancellationToken);
        return Ok();
    }
}
