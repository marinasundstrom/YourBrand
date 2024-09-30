using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Ticketing.Application.Common;
using YourBrand.Ticketing.Application.Features.Teams.Commands;
using YourBrand.Ticketing.Application.Features.Teams.Queries;

namespace YourBrand.Ticketing.Application.Features.Teams;

[ApiController]
[ApiVersion("1")]
[Route("v{version:apiVersion}/[controller]")]
[Authorize]
public class TeamsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ItemsResult<TeamDto>> GetTeams(string organizationId, int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetTeamsQuery(organizationId, page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<TeamDto?> GetTeam(string organizationId, string id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetTeamQuery(organizationId, id), cancellationToken);
    }

    [HttpPost]
    public async Task<TeamDto> CreateTeam(string organizationId, CreateTeamDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateTeamCommand(organizationId, null!, dto.Name, dto.Description), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task<TeamDto> UpdateTeam(string organizationId, string id, UpdateTeamDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new UpdateTeamCommand(organizationId, id, dto.Name, dto.Description), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteTeam(string organizationId, string id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTeamCommand(organizationId, id), cancellationToken);
    }

    [HttpPost("{id}/Members")]
    public async Task AddMember(string organizationId, string id, AddMemberDto dto, CancellationToken cancellationToken)
    {
        await mediator.Send(new AddTeamMemberCommand(organizationId, id, dto.UserId), cancellationToken);
    }

    [HttpDelete("{id}/Members/{userId}")]
    public async Task RemoveMember(string organizationId, string id, string userId, CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveTeamMemberCommand(organizationId, id, userId), cancellationToken);
    }

    [HttpGet("{id}/Memberships")]
    public async Task<ItemsResult<TeamMembershipDto>> GetMemberships(string organizationId, string id, int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetTeamMembershipsQuery(organizationId, id, page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }
}

public record CreateTeamDto(string Name, string? Description);

public record UpdateTeamDto(string Name, string? Description);

public record AddMemberDto(string UserId);