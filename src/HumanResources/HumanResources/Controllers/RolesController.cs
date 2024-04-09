using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.HumanResources.Application.Common.Models;
using YourBrand.HumanResources.Application.Persons;
using YourBrand.HumanResources.Application.Persons.Queries;

namespace YourBrand.HumanResources;

[Route("[controller]")]
[ApiController]
[Authorize]
public class RolesController(IMediator mediator) : Controller
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<RoleDto>>> GetRoles(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, HumanResources.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetRolesQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }
}