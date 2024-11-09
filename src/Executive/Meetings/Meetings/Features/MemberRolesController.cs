using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Meetings.Features.Groups;
using YourBrand.Meetings.Features.Groups.Queries;
using YourBrand.Meetings.Features.Queries;
using YourBrand.Meetings.Models;

namespace YourBrand.Meetings.Features;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/MemberRoles")]
[Authorize]
public sealed partial class MemberRolesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResult<AttendeeRoleDto>))]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesDefaultResponseType]
    public async Task<PagedResult<MemberRoleDto>> GetMemberRoles(string organizationId, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
        => await mediator.Send(new GetMemberRoles(organizationId, page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);
}