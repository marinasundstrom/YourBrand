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
        private readonly IUserContext _userContext;

        public SyncDataCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher, IUserContext userContext)
        {
            _context = context;
            _eventPublisher = eventPublisher;
            _userContext = userContext;
        }

        public async Task Handle(SyncDataCommand request, CancellationToken cancellationToken)
        {
            var organizations = await _context.Organizations
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach (var organization in organizations)
            {
                await _eventPublisher.PublishEvent(new OrganizationCreated(organization.Id, organization.Name, _userContext.UserId));
            }

            var users = await _context.Persons
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach (var user in users)
            {
                await _eventPublisher.PublishEvent(new PersonCreated(user.Id, _userContext.UserId));
            }
        }
    }
}