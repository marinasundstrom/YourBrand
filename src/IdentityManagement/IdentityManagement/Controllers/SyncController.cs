using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.IdentityManagement.Application.Users.Commands;

namespace YourBrand.IdentityManagement;

[Route("[controller]")]
[ApiController]
//[Authorize]
public class SyncController : Controller
{
    private readonly IMediator _mediator;

    public SyncController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> SyncData(CancellationToken cancellationToken)
    {
        await _mediator.Send(new SyncDataCommand(), cancellationToken);
        return Ok();
    }
}