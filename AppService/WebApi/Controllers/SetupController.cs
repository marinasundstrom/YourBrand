
using YourBrand.Application.Common.Models;
using YourBrand.Application.Search;
using YourBrand.Application.Search.Commands;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YourBrand.Application.Setup;

namespace YourBrand.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class SetupController : Controller
{
    private readonly IMediator _mediator;

    public SetupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Search(SetupRequest request, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new SetupCommand(request.OrganizationName, request.Email, request.Password), cancellationToken));
    }
}


public record SetupRequest(string OrganizationName, string Email, string Password);