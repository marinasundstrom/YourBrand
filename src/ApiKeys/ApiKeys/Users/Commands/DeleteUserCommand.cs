
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Domain.Exceptions;

namespace YourBrand.ApiKeys.Application.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IApiKeysContext _context;

        public DeleteUserCommandHandler(IApiKeysContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                        .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}