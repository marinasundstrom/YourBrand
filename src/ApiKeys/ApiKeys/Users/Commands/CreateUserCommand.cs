
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Domain.Entities;

namespace YourBrand.ApiKeys.Application.Users.Commands;

public record CreateUserCommand(string? Id, string FirstName, string LastName, string? DisplayName, string Email) : IRequest<UserDto>
{
    public class CreateUserCommand1Handler : IRequestHandler<CreateUserCommand, UserDto>
    {
        readonly IApiKeysContext _context;

        public CreateUserCommand1Handler(IApiKeysContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

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

            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.Email, user.Created, user.LastModified);
        }
    }
}