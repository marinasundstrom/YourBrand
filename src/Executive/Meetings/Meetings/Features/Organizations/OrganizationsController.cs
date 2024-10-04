using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Features;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features.Organizations;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
public sealed class OrganizationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<OrganizationDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<OrganizationDto>> GetOrganizations(int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetOrganizations(page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);

    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrganizationDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
    [ProducesDefaultResponseType]
    public async Task<ActionResult<OrganizationDto>> CreateOrganization(CreateOrganizationDto request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateOrganization(request.Name, request.Email, null!), cancellationToken);
        return result.GetValue();
    }
}

public sealed record CreateOrganizationDto(string Name, string Email);