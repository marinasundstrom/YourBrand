
using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using YourBrand.Application.Setup;

namespace YourBrand.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class SetupController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Setup(SetupRequest request, CancellationToken cancellationToken = default)
    {
        await mediator.Send(new SetupCommand(request.OrganizationName, request.Email, request.Password), cancellationToken);
        return Ok();
    }
}


public record SetupRequest(string OrganizationName, string Email, string Password);