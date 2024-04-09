
using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Application.Common.Models;
using YourBrand.Application.Search;
using YourBrand.Application.Search.Commands;

namespace YourBrand.WebApi.Controllers;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class SearchController(IMediator mediator) : Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(Results<SearchResultItem>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Results<SearchResultItem>>> Search(string searchText,
        int page = 1, int pageSize = 5, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new SearchCommand(searchText, page - 1, pageSize, sortBy, sortDirection), cancellationToken));
    }
}