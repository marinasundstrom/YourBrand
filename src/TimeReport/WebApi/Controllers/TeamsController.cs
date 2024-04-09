using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Teams;
using YourBrand.TimeReport.Application.Teams.Commands;
using YourBrand.TimeReport.Application.Teams.Queries;

namespace YourBrand.TimeReport.WebApi.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AuthSchemes.Default)]
public class TeamsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ItemsResult<TeamDto>> GetTeams(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetTeamsQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<TeamDto?> GetTeam(string id, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetTeamQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<TeamDto> CreateTeam(CreateTeamDto dto, CancellationToken cancellationToken)
    {
        //return await _mediator.Send(new CreateTeamCommand(dto.Name, dto.Description), cancellationToken);

        return null!;
    }

    [HttpPut("{id}")]
    public async Task<TeamDto> UpdateTeam(string id, UpdateTeamDto dto, CancellationToken cancellationToken)
    {
        return await mediator.Send(new UpdateTeamCommand(id, dto.Name, dto.Description), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteTeam(string id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteTeamCommand(id), cancellationToken);
    }

    [HttpPost("{id}/Members")]
    public async Task AddMember(string id, AddMemberDto dto, CancellationToken cancellationToken)
    {
        await mediator.Send(new AddTeamMemberCommand(id, dto.UserId), cancellationToken);
    }

    [HttpDelete("{id}/Members/{userId}")]
    public async Task RemoveMember(string id, string userId, CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveTeamMemberCommand(id, userId), cancellationToken);
    }

    [HttpGet("{id}/Memberships")]
    public async Task<ItemsResult<TeamMembershipDto>> GetMemberships(string id, int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetTeamMembershipsQuery(id, page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }
}

public record CreateTeamDto(string Name, string? Description);

public record UpdateTeamDto(string Name, string? Description);

public record AddMemberDto(string UserId);