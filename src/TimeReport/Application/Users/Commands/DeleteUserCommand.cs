
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Domain.Exceptions;

namespace YourBrand.TimeReport.Application.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest
{
    public class DeleteUserCommandHandler(ITimeReportContext context) : IRequestHandler<DeleteUserCommand>
    {
        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                        .AsSplitQuery()
                        .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            context.Users.Remove(user);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}