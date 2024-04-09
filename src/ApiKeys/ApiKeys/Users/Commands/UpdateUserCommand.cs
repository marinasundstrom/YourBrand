
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Domain.Exceptions;

namespace YourBrand.ApiKeys.Application.Users.Commands;

public record UpdateUserCommand(string UserId, string FirstName, string LastName, string? DisplayName, string Email) : IRequest<UserDto>
{
    public class UpdateUserCommandHandler(IApiKeysContext context) : IRequestHandler<UpdateUserCommand, UserDto>
    {
        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.DisplayName = request.DisplayName;
            user.Email = request.Email;

            await context.SaveChangesAsync(cancellationToken);

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.Email, user.Created, user.LastModified);
        }
    }
}