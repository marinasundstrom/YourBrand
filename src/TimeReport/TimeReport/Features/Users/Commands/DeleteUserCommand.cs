
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Features.Common.Interfaces;

namespace YourBrand.TimeReport.Features.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest<Result>
{
    public class DeleteUserCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteUserCommand, Result>
    {
        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                        .AsSplitQuery()
                        .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
               return new UserNotFound(request.UserId);
            }

            context.Users.Remove(user);

            await context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}