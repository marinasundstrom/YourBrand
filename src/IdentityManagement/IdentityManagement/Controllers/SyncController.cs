using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.IdentityManagement.Application.Users.Commands;

namespace YourBrand.IdentityManagement;

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