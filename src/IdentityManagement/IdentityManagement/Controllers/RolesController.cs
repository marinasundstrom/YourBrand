using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.IdentityManagement.Application.Common.Models;
using YourBrand.IdentityManagement.Application.Users;
using YourBrand.IdentityManagement.Application.Users.Queries;

namespace YourBrand.IdentityManagement;

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
    public async Task<ActionResult<ItemsResult<RoleDto>>> GetRoles(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, IdentityManagement.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetRolesQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }
}