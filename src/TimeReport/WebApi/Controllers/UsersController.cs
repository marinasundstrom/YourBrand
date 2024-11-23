using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.ApiKeys;
using YourBrand.TimeReport.Application.Common.Models;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Application.Users;
using YourBrand.TimeReport.Application.Users.Commands;
using YourBrand.TimeReport.Application.Users.Queries;

namespace YourBrand.TimeReport.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<UserDto>>> GetUsers(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, TimeReport.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        var r = await mediator.Send(new GetUsersQuery(page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> GetUser(string id, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new GetUserQuery(id), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpPost]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new CreateUserCommand(null!, createUserDto.FirstName, createUserDto.LastName, createUserDto.DisplayName, createUserDto.SSN, createUserDto.Email), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpPut("{id}/Details")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> UpdateUser(string id, UpdateUserDetailsDto updateUserDetailsDto, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new UpdateUserCommand(id, updateUserDetailsDto.FirstName, updateUserDetailsDto.LastName, updateUserDetailsDto.DisplayName, updateUserDetailsDto.SSN, updateUserDetailsDto.Email), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.AdministratorManager)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> DeleteUser(string id, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpGet("{id}/Memberships")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<ProjectMembershipDto>>> GetProjectMemberships(string id, int page = 0, int pageSize = 10, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        var r = await mediator.Send(new GetUserProjectMembershipsQuery(id, page, pageSize, sortBy, sortDirection), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpGet("{id}/Statistics")]
    public async Task<ActionResult<Data>> GetStatistics(string id, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var r = await mediator.Send(new GetUserStatisticsQuery(id, from, to), cancellationToken);
        return this.HandleResult(r);
    }

    [HttpGet("{id}/Statistics/Summary")]
    public async Task<ActionResult<StatisticsSummary>> GetStatisticsSummary(string id, CancellationToken cancellationToken)
    {
        var r = await mediator.Send(new GetUserStatisticsSummaryQuery(id), cancellationToken);
        return this.HandleResult(r);
    }
}

public record class CreateUserDto(string FirstName, string LastName, string? DisplayName, string SSN, string Email);

public record class UpdateUserDetailsDto(string FirstName, string LastName, string? DisplayName, string SSN, string Email);