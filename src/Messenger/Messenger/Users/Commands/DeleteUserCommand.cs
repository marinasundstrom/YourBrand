
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Domain.Exceptions;

namespace YourBrand.Messenger.Application.Users.Commands;

public class DeleteUserCommand(string userId) : IRequest
{
    public string UserId { get; } = userId;

    public class DeleteUserCommandHandler(IMessengerContext context) : IRequestHandler<DeleteUserCommand>
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