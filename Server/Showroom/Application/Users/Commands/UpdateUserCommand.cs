
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourCompany.Showroom.Application.Common.Interfaces;
using YourCompany.Showroom.Domain.Exceptions;

namespace YourCompany.Showroom.Application.Users.Commands;

public class UpdateUserCommand : IRequest<UserDto>
{
    public UpdateUserCommand(string userId, string firstName, string lastName, string? displayName, string ssn, string email)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        DisplayName = displayName;
        Ssn = ssn;
        Email = email;
    }

    public string UserId { get; }

    public string FirstName { get; }

    public string LastName { get; }

    public string? DisplayName { get; }

    public string Ssn { get; }

    public string Email { get; }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    { 
        readonly IShowroomContext _context;

        public UpdateUserCommandHandler(IShowroomContext context)
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
            user.SSN = request.Ssn;
            user.Email = request.Email;

            await _context.SaveChangesAsync(cancellationToken);

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.LastModified);
        }
    }
}