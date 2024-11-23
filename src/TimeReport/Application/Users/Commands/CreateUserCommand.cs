
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Users.Commands;

public record CreateUserCommand(string? Id, string FirstName, string LastName, string? DisplayName, string Ssn, string Email) : IRequest<Result<UserDto>>
{
    public class CreateUserCommandHandler(ITimeReportContext context) : IRequestHandler<CreateUserCommand, Result<UserDto>>
    {
        public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .Include(x => x.Teams)
                .Include(x => x.Organizations)
                .FirstOrDefaultAsync(u => u.Id == request.Id);

            if (user is not null)
            {
                return user.ToDto();
            }

            user = new User(request.Id ?? Guid.NewGuid().ToString())
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DisplayName = request.DisplayName,
                SSN = request.Ssn,
                Email = request.Email
            };

            context.Users.Add(user);

            await context.SaveChangesAsync(cancellationToken);

            return user.ToDto();
        }
    }
}