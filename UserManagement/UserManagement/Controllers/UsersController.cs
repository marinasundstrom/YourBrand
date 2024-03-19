
using AspNetCore.Authentication.ApiKey;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using YourBrand.UserManagement.Application.Common.Models;
using YourBrand.UserManagement.Application.Users;
using YourBrand.UserManagement.Application.Users.Commands;
using YourBrand.UserManagement.Application.Users.Queries;
using YourBrand.UserManagement.Domain.Exceptions;

namespace YourBrand.UserManagement;

[Route("[controller]")]
[ApiController]
[Authorize]
public class UsersController : Controller
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<UserDto>>> GetUsers(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, UserManagement.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
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

    [HttpGet("{id}/Roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<RoleDto>>> GetUserRoles(string id, int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, UserManagement.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
    {
        return Ok(await _mediator.Send(new GetUserRolesQuery(id, page, pageSize, searchString, sortBy, sortDirection), cancellationToken));

    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUserDto, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _mediator.Send(new CreateUserCommand(createUserDto.OrganizationId, createUserDto.FirstName, createUserDto.LastName, createUserDto.DisplayName, createUserDto.Role, createUserDto.Email), cancellationToken);

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
            var user = await _mediator.Send(new UpdateOrganizationCommand(id, updateUserDetailsDto.FirstName, updateUserDetailsDto.LastName, updateUserDetailsDto.DisplayName, updateUserDetailsDto.Title, updateUserDetailsDto.SSN, updateUserDetailsDto.Email, updateUserDetailsDto.ReportsTo), cancellationToken);

            return Ok(user);
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }
    
    [HttpPost("{id}/Role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> UpdateRole(string id, UpdateUserRoleDto updateUserRoleDtoDto, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new UpdateUserRoleCommand(id, updateUserRoleDtoDto.Role), cancellationToken);

            return Ok();
        }
        catch (UserNotFoundException)
        {
            return NotFound();
        }
    }


    [HttpPost("{id}/ChangePassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ChangePassword(string id, ChangePasswordDto changePasswordDto, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new UpdateUserPasswordCommand(id, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword), cancellationToken);

            return Ok();
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
}

public record class CreateUserDto(string OrganizationId, string FirstName, string LastName, string? DisplayName, string Title, string Role, string SSN, string Email, string? ReportsTo, string Password);

public record class UpdateUserDetailsDto(string FirstName, string LastName, string? DisplayName, string Title, string SSN, string Email, string? ReportsTo);

public record class ChangePasswordDto(string CurrentPassword, string NewPassword);

public record class UpdateUserRoleDto(string Role);