using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Application.Users.Commands;

public record SyncDataCommand() : IRequest
{
    public sealed class SyncDataCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<SyncDataCommand>
    {
        public async Task Handle(SyncDataCommand request, CancellationToken cancellationToken)
        {
            var organizations = await context.Organizations
                .Include(p => p.Tenant)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach (var organization in organizations)
            {
                await eventPublisher.PublishEvent(new OrganizationCreated(organization.Id, organization.Tenant.Id, organization.Name));
            }

            var users = await context.Users
                .Include(p => p.Tenant)
                .Include(p => p.Organizations)
                .OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach (var user in users)
            {
                await eventPublisher.PublishEvent(new UserCreated(user.Id, user.Tenant!.Id, user.Organization?.Id ?? "x"));
            }

            var organizationUsers = await context.OrganizationUsers
                //.OrderBy(p => p.Created)
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            foreach (var organizationUser in organizationUsers)
            {
                await eventPublisher.PublishEvent(new OrganizationUserAdded(organizationUser.TenantId, organizationUser.OrganizationId, organizationUser.UserId));
            }
        }
    }
}