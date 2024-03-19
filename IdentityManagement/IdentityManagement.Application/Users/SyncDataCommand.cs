using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.Identity;

namespace YourBrand.IdentityManagement.Application.Users.Commands;

public record SyncDataCommand() : IRequest
{
    public class SyncDataCommandHandler : IRequestHandler<SyncDataCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICurrentUserService _currentUserService;

        public SyncDataCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher, ICurrentUserService currentUserService)
        {
            _context = context;
            _eventPublisher = eventPublisher;
            _currentUserService = currentUserService;
        }

        public async Task Handle(SyncDataCommand request, CancellationToken cancellationToken)
        {
            var organizations = await _context.Organizations
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach(var organization in organizations) 
            {
                await _eventPublisher.PublishEvent(new OrganizationCreated(organization.Id, organization.Name, _currentUserService.UserId));
            }

            var users = await _context.Users
                .Include(p => p.Organization)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach(var user in users) 
            {
                await _eventPublisher.PublishEvent(new UserCreated(user.Id, user.Organization!.Id, _currentUserService.UserId));
            }
        }
    }
}