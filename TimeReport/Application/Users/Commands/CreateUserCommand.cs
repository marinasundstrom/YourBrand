
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Users.Commands;

public record CreateUserCommand(string? Id, string FirstName, string LastName, string? DisplayName, string Ssn, string Email) : IRequest<UserDto>
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly ITimeReportContext _context;

        public CreateUserCommandHandler(ITimeReportContext context)
        {
            _context = context;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(x => x.Teams)
                .FirstOrDefaultAsync(u => u.Id == request.Id);

            if(user is not null) 
            {
                return user.ToDto();
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

            return user.ToDto();
        }
    }
}