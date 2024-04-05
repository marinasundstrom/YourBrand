using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Exceptions;

namespace YourBrand.IdentityManagement.Application.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserContext _userContext;
        private readonly IEventPublisher _eventPublisher;

        public DeleteUserCommandHandler(IApplicationDbContext context, IUserContext userContext, IEventPublisher eventPublisher)
        {
            _context = context;
            _userContext = userContext;
            _eventPublisher = eventPublisher;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(p => p.Roles)
                .FirstOrDefaultAsync(p => p.Id == request.UserId);

            if (user is null)
            {
                throw new UserNotFoundException(request.UserId);
            }

            _context.Users.Remove(user);

            await _context.SaveChangesAsync(cancellationToken);

            await _eventPublisher.PublishEvent(new UserDeleted(user.Id, _userContext.UserId));

        }
    }
}