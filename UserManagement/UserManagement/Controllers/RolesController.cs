
using AspNetCore.Authentication.ApiKey;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.UserManagement.Application.Common.Models;
using YourBrand.UserManagement.Application.Users;
using YourBrand.UserManagement.Application.Users.Commands;
using YourBrand.UserManagement.Application.Users.Queries;
using YourBrand.UserManagement.Domain.Exceptions;

namespace YourBrand.UserManagement;

[Route("[controller]")]
[ApiController]
[Authorize]
public class RolesController : Controller
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<RoleDto>>> GetRoles(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, UserManagement.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetRolesQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }
}