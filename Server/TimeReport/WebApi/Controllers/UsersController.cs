
using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TimeReport.Application;
using TimeReport.Application.Common.Interfaces;
using TimeReport.Application.Common.Models;
using TimeReport.Application.Projects;
using TimeReport.Application.Users;
using TimeReport.Application.Users.Commands;
using TimeReport.Application.Users.Queries;
using TimeReport.Domain.Exceptions;

namespace TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<UserDto>>> GetUsers(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, TimeReport.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetUsersQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> GetUser(string id, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserQuery(id), cancellationToken);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _mediator.Send(new CreateUserCommand(createUserDto.FirstName, createUserDto.LastName, createUserDto.DisplayName, createUserDto.SSN, createUserDto.Email), cancellationToken);

            return Ok(user);
        }
        catch (Exception)
        {
            throw;
        }
    }

    [HttpPut("{id}/Details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> UpdateUser(string id, UpdateUserDetailsDto updateUserDetailsDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _mediator.Send(new UpdateUserCommand(id, updateUserDetailsDto.FirstName, updateUserDetailsDto.LastName, updateUserDetailsDto.DisplayName, updateUserDetailsDto.SSN, updateUserDetailsDto.Email), cancellationToken);

            return Ok(user);
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new DeleteUserCommand(id), cancellationToken);

            return Ok();
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Memberships")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ProjectMembershipDto>>> GetProjectMemberships(string id, int page = 0, int pageSize = 10, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _mediator.Send(new GetUserProjectMembershipsQuery(id, page, pageSize, sortBy, sortDirection), cancellationToken));
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Statistics")]
    public async Task<ActionResult<Data>> GetStatistics(string id, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _mediator.Send(new GetUserStatisticsQuery(id, from, to), cancellationToken));
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("{id}/Statistics/Summary")]
    public async Task<ActionResult<StatisticsSummary>> GetStatisticsSummary(string id, CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _mediator.Send(new GetUserStatisticsSummaryQuery(id), cancellationToken));
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }
}

public record class CreateUserDto(string FirstName, string LastName, string? DisplayName, string SSN, string Email);

public record class UpdateUserDetailsDto(string FirstName, string LastName, string? DisplayName, string SSN, string Email);