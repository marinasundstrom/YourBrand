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
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ItemsResult<TeamDto>> GetTeams(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetTeamsQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<TeamDto?> GetTeam(string id, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetTeamQuery(id), cancellationToken);
    }

    [HttpPost]
    public async Task<TeamDto> CreateTeam(CreateTeamDto dto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new CreateTeamCommand(dto.Name), cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task<TeamDto> UpdateTeam(string id, UpdateTeamDto dto, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new UpdateTeamCommand(id, dto.Name), cancellationToken);
    }

    [HttpDelete("{id}")]
    public async Task DeleteTeam(string id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteTeamCommand(id), cancellationToken);
    }

    [HttpPost("{id}/Members")]
    public async Task AddMember(string id, string userId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new AddTeamMemberCommand(id, userId), cancellationToken);
    }

    [HttpDelete("{id}/Members/{userId}")]
    public async Task RemoveMember(string id, string userId, CancellationToken cancellationToken)
    {
        await _mediator.Send(new RemoveTeamMemberCommand(id, userId), cancellationToken);
    }

    [HttpGet("{id}/Memberships")]
    public async Task<ItemsResult<TeamMembershipDto>> GetMemberships(string id, int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await _mediator.Send(new GetTeamMembershipsQuery(id, page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }
}

public record CreateTeamDto(string Name);

public record UpdateTeamDto(string Name);

