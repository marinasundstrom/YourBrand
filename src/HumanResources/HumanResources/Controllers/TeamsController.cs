
using AspNetCore.Authentication.ApiKey;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.HumanResources.Application.Common.Models;
using YourBrand.HumanResources.Application.Teams;
using YourBrand.HumanResources.Application.Teams.Commands;
using YourBrand.HumanResources.Application.Teams.Queries;
using YourBrand.HumanResources.Domain.Exceptions;

namespace YourBrand.HumanResources;

[Route("[controller]")]
[ApiController]
[Authorize]
public class TeamsController(IMediator mediator) : Controller
{
    private const string AuthSchemes =
        JwtBearerDefaults.AuthenticationScheme + "," +
        ApiKeyDefaults.AuthenticationScheme;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<TeamDto>>> GetTeams(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, HumanResources.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await mediator.Send(new GetTeamsQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TeamDto>> GetTeam(string id, CancellationToken cancellationToken)
    {
        var person = await mediator.Send(new GetTeamQuery(id), cancellationToken);

        if (person is null)
        {
            return NotFound();
        }

        return Ok(person);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TeamDto>> CreateTeam(CreateTeamDto createTeamDto, CancellationToken cancellationToken)
    {
        try
        {
            var team = await mediator.Send(new CreateTeamCommand(createTeamDto.Name, createTeamDto.Description, createTeamDto.OrganizationId), cancellationToken);

            return Ok(team);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TeamDto>> UpdateTeam(string id, UpdateTeamDto updateTeamDto, CancellationToken cancellationToken)
    {
        try
        {
            var team = await mediator.Send(new UpdateTeamCommand(id, updateTeamDto.Name, updateTeamDto.Description), cancellationToken);

            return Ok(team);
        }
        catch (PersonNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteTeam(string id, CancellationToken cancellationToken)
    {
        try
        {
            await mediator.Send(new DeleteTeamCommand(id), cancellationToken);

            return Ok();
        }
        catch (PersonNotFoundException)
        {
            return NotFound();
        }
    }


    [HttpPost("{id}/Members")]
    public async Task AddMember(string id, AddMemberDto dto, CancellationToken cancellationToken)
    {
        await mediator.Send(new AddTeamMemberCommand(id, dto.PersonId), cancellationToken);
    }

    [HttpDelete("{id}/Members/{personId}")]
    public async Task RemoveMember(string id, string personId, CancellationToken cancellationToken)
    {
        await mediator.Send(new RemoveTeamMemberCommand(id, personId), cancellationToken);
    }

    [HttpGet("{id}/Memberships")]
    public async Task<ItemsResult<TeamMembershipDto>> GetMemberships(string id, int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetTeamMembershipsQuery(id, page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }
}

public record class CreateTeamDto(string Name, string Description, string OrganizationId);

public record class UpdateTeamDto(string Name, string Description);

public record AddMemberDto(string PersonId);