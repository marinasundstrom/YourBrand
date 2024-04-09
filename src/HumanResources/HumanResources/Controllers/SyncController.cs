using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.HumanResources.Application.Users.Commands;

namespace YourBrand.HumanResources;

[Route("[controller]")]
[ApiController]
//[Authorize]
public class SyncController(IMediator mediator) : Controller
{
    [HttpPost]
    public async Task<ActionResult> SyncData(CancellationToken cancellationToken)
    {
        await mediator.Send(new SyncDataCommand(), cancellationToken);
        return Ok();
    }
}