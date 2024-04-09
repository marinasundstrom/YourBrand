
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Exceptions;

namespace YourBrand.Application.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest
{
    public class DeleteUserCommandHandler(IAppServiceContext context) : IRequestHandler<DeleteUserCommand>
    {
        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users
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