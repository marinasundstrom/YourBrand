using MediatR;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using YourBrand.UserManagement.Application.Common.Interfaces;
using YourBrand.UserManagement.Contracts;
using YourBrand.UserManagement.Domain.Entities;
using YourBrand.UserManagement.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.UserManagement.Application.Users.Commands;

public record DeleteUserCommand(string UserId) : IRequest
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEventPublisher _eventPublisher;

        public DeleteUserCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService, IEventPublisher eventPublisher)
        {
            _context = context;
            _currentUserService = currentUserService;
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

            await _eventPublisher.PublishEvent(new UserDeleted(user.Id, _currentUserService.UserId));

        }
    }
}