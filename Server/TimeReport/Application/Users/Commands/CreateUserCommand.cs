
using MediatR;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Domain.Entities;

namespace TimeReport.Application.Users.Commands;

public class CreateUserCommand : IRequest<UserDto>
{
    public CreateUserCommand(string firstName, string lastName, string? displayName, string ssn, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        DisplayName = displayName;
        Ssn = ssn;
        Email = email;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public string? DisplayName { get; }

    public string Ssn { get; }

    public string Email { get; }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly ITimeReportContext _context;

        public CreateUserCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                SSN = request.Ssn,
                Email = request.Email
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync(cancellationToken);

            return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.Deleted);
        }
    }
}