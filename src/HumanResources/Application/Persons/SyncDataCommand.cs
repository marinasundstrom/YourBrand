using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Users.Commands;

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

            var users = await _context.Persons
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach(var user in users) 
            {
                await _eventPublisher.PublishEvent(new PersonCreated(user.Id, _currentUserService.UserId));
            }

        }
    }
}