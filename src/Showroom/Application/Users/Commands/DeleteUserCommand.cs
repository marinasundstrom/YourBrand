
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Domain.Exceptions;

namespace YourBrand.Showroom.Application.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest
{
    public class DeleteUserCommandHandler(IShowroomContext context) : IRequestHandler<DeleteUserCommand>
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