
using AspNetCore.Authentication.ApiKey;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Skynet.IdentityService.Application.Common.Models;
using Skynet.IdentityService.Application.Users;
using Skynet.IdentityService.Application.Users.Commands;
using Skynet.IdentityService.Application.Users.Queries;
using Skynet.IdentityService.Domain.Exceptions;

namespace Skynet.IdentityService;

[Route("[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AuthSchemes)]
public class RolesController : Controller
{
    private const string AuthSchemes =
        JwtBearerDefaults.AuthenticationScheme + "," +
        ApiKeyDefaults.AuthenticationScheme;

    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<RoleDto>>> GetRoles(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, IdentityService.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetRolesQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }
}