
using AspNetCore.Authentication.ApiKey;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.HumanResources.Application.Common.Models;
using YourBrand.HumanResources.Application.Teams;
using YourBrand.HumanResources.Application.Teams.Queries;
using YourBrand.HumanResources.Application.Teams.Commands;
using YourBrand.HumanResources.Domain.Exceptions;

namespace YourBrand.HumanResources;

[Route("[controller]")]
[ApiController]
public class TeamsController : Controller
{
    private const string AuthSchemes =
        JwtBearerDefaults.AuthenticationScheme + "," +
        ApiKeyDefaults.AuthenticationScheme;

    private readonly IMediator _mediator;

    public TeamsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<TeamDto>>> GetTeams(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, HumanResources.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetTeamsQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TeamDto>> GetTeam(string id, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetTeamQuery(id), cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<TeamDto>> CreateTeam(CreateTeamDto createTeanDto, CancellationToken cancellationToken)
    {
        try
        {
            var team = await _mediator.Send(new CreateTeamCommand(createTeanDto.Name, createTeanDto.Description), cancellationToken);

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
            var team = await _mediator.Send(new UpdateTeamCommand(id, updateTeamDto.Name, updateTeamDto.Description), cancellationToken);

            return Ok(team);
        }
        catch (UserNotFoundException)
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
            await _mediator.Send(new DeleteTeamCommand(id), cancellationToken);

            return Ok();
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }
}

public record class CreateTeamDto(string Name, string Description);

public record class UpdateTeamDto(string Name, string Description);