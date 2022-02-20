
using AspNetCore.Authentication.ApiKey;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Skynet.IdentityService.Application.Common.Models;
using Skynet.IdentityService.Application.Users;
using Skynet.IdentityService.Application.Users.Commands;
using Skynet.IdentityService.Application.Users.Queries;
using Skynet.IdentityService.Domain.Exceptions;

namespace Skynet.IdentityService;

[Route("[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = AuthSchemes)]
public class UsersController : Controller
{
    private const string AuthSchemes =
        JwtBearerDefaults.AuthenticationScheme + "," +
        ApiKeyDefaults.AuthenticationScheme;

    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<ItemsResult<UserDto>>> GetUsers(int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, IdentityService.Application.Common.Models.SortDirection? sortDirection = null, CancellationToken cancellationToken = default)
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
            var user = await _mediator.Send(new CreateUserCommand(createUserDto.FirstName, createUserDto.LastName, createUserDto.DisplayName, createUserDto.SSN, createUserDto.Email, null), cancellationToken);

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
            var user = await _mediator.Send(new UpdateUserDetailsCommand(id, updateUserDetailsDto.FirstName, updateUserDetailsDto.LastName, updateUserDetailsDto.DisplayName, updateUserDetailsDto.SSN, updateUserDetailsDto.Email), cancellationToken);

            return Ok(user);
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

public record class CreateUserDto(string FirstName, string LastName, string? DisplayName, string SSN, string Email);

public record class UpdateUserDetailsDto(string FirstName, string LastName, string? DisplayName, string SSN, string Email);

public record class ChangePasswordDto(string CurrentPassword, string NewPassword);