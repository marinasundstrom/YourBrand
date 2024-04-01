
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Domain.Exceptions;

namespace YourBrand.Messenger.Application.Users.Commands;

public class UpdateUserCommand : IRequest<UserDto>
{
    public UpdateUserCommand(string userId, string firstName, string lastName, string? displayName, string email)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        DisplayName = displayName;
        Email = email;
    }

    public string UserId { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string? DisplayName { get; }

    public string Email { get; }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        readonly IMessengerContext _context;

        public UpdateUserCommandHandler(IMessengerContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.DisplayName = request.DisplayName;
            user.Email = request.Email;

            await _context.SaveChangesAsync(cancellationToken);

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.Email, user.Created, user.LastModified);
        }
    }
}