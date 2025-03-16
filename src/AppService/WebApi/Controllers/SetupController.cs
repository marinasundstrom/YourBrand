
using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

using YourBrand.Application.Setup;

namespace YourBrand.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public class SetupController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Setup(SetupRequest request, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new SetupCommand(request.TenantName, request.OrganizationName, request.FirstName, request.LastName, request.Email, request.Password), cancellationToken);
        if(result.Status == SetupStatusCode.Failed && result.Reason == SetupFailReason.EmailAddressAlreadyRegistered)
        {
            return Problem("Email address already existing", statusCode: StatusCodes.Status400BadRequest);
        }
        return Ok();
    }
}


public record SetupRequest(string? TenantName, string OrganizationName, string FirstName, string LastName, string Email, string Password);