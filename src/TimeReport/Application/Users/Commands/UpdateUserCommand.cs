
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Application.Users.Commands;

public record UpdateUserCommand(string UserId, string FirstName, string LastName, string? DisplayName, string Ssn, string Email) : IRequest<Result<UserDto>>
{
    public class UpdateUserCommandHandler(ITimeReportContext context) : IRequestHandler<UpdateUserCommand, Result<UserDto>>
    {
        public async Task<Result<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
               return new UserNotFound(request.UserId);
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.DisplayName = request.DisplayName;
            user.SSN = request.Ssn;
            user.Email = request.Email;

            await context.SaveChangesAsync(cancellationToken);

            return user.ToDto();
        }
    }
}