
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.Users.Commands;

public record CreateUserCommand(string? Id, string FirstName, string LastName, string? DisplayName, string Ssn, string Email) : IRequest<UserDto>
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    { 
        readonly IShowroomContext _context;

        public CreateUserCommandHandler(IShowroomContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

            if(user is not null) 
            {
                return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.LastModified);
            }

            user = new User
            {
                Id = request.Id ?? Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                SSN = request.Ssn,
                Email = request.Email
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.LastModified);
        }
    }
}