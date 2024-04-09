
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Domain.Exceptions;

namespace YourBrand.Messenger.Application.Users.Commands;

public class UpdateUserCommand(string userId, string firstName, string lastName, string? displayName, string email) : IRequest<UserDto>
{
    public string UserId { get; } = userId;

    public string FirstName { get; } = firstName;

    public string LastName { get; } = lastName;

    public string? DisplayName { get; } = displayName;

    public string Email { get; } = email;

    public class UpdateUserCommandHandler(IMessengerContext context) : IRequestHandler<UpdateUserCommand, UserDto>
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