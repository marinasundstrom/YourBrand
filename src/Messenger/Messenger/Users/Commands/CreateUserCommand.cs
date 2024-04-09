
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Contracts;
using YourBrand.Messenger.Domain.Entities;

namespace YourBrand.Messenger.Application.Users.Commands;

public class CreateUserCommand(string? id, string firstName, string lastName, string? displayName, string email) : IRequest<UserDto>
{
    public string? Id { get; } = id;

    public string FirstName { get; } = firstName;

    public string LastName { get; } = lastName;

    public string? DisplayName { get; } = displayName;

    public string Email { get; } = email;

    public class CreateUserCommand1Handler(IMessengerContext context) : IRequestHandler<CreateUserCommand, UserDto>
    {
        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

            if (user is not null)
            {
                return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.Email, user.Created, user.LastModified);
            }

            user = new User
            {
                Id = request.Id ?? Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                Email = request.Email
            };

            context.Users.Add(user);

            await context.SaveChangesAsync(cancellationToken);

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.Email, user.Created, user.LastModified);
        }
    }
}