
using AspNetCore.Authentication.ApiKey;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.HumanResources.Application.Common.Models;
using YourBrand.HumanResources.Application.Teams;
using YourBrand.HumanResources.Application.Teams.Queries;
using YourBrand.HumanResources.Application.Teams.Commands;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.HumanResources.Application.Users.Commands;

namespace YourBrand.HumanResources;

[Route("[controller]")]
[ApiController]
[Authorize]
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
