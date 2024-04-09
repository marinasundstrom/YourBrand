using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Exceptions;

namespace YourBrand.IdentityManagement.Application.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest
{
    public class DeleteUserCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<DeleteUserCommand>
    {
        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users
                .Include(p => p.Roles)
                .FirstOrDefaultAsync(p => p.Id == request.UserId);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            context.Users.Remove(user);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new UserDeleted(user.Id));

        }
    }
}