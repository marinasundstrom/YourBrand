using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.IdentityService.Contracts;

namespace YourBrand.IdentityService.Application.Users.Commands;

public record SyncUsersCommand() : IRequest
{
    public class SyncUsersCommandHandler : IRequestHandler<SyncUsersCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICurrentUserService _currentUserService;

        public SyncUsersCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher, ICurrentUserService currentUserService)
        {
            _context = context;
            _eventPublisher = eventPublisher;
            _currentUserService = currentUserService;
        }

        public async Task Handle(SyncUsersCommand request, CancellationToken cancellationToken)
        {
            var users = await _context.Users
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach(var user in users) 
            {
                await _eventPublisher.PublishEvent(new UserCreated(user.Id, _currentUserService.UserId));
            }

        }
    }
}