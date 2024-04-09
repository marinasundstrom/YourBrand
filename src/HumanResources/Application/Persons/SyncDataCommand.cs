using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Users.Commands;

public record SyncDataCommand() : IRequest
{
    public sealed class SyncDataCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<SyncDataCommand>
    {
        public async Task Handle(SyncDataCommand request, CancellationToken cancellationToken)
        {
            var organizations = await context.Organizations
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach (var organization in organizations)
            {
                await eventPublisher.PublishEvent(new OrganizationCreated(organization.Id, organization.Name));
            }

            var users = await context.Persons
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach (var user in users)
            {
                await eventPublisher.PublishEvent(new PersonCreated(user.Id));
            }
        }
    }
}