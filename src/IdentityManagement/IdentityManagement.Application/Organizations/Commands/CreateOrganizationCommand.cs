using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Domain.Entities;

using OrganizationCreated = YourBrand.IdentityManagement.Contracts.OrganizationCreated;

namespace YourBrand.IdentityManagement.Application.Organizations.Commands;

public record CreateOrganizationCommand(string Name, string? FriendlyName) : IRequest<OrganizationDto>
{
    public sealed class CreateOrganizationCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<CreateOrganizationCommand, OrganizationDto>
    {
        public async Task<OrganizationDto> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = new Organization(
                 request.Name,
                 request.FriendlyName ?? request.Name.ToLower().Replace(' ', '-'));

            context.Organizations.Add(organization);

            await context.SaveChangesAsync(cancellationToken);

            organization = await context.Organizations
               .AsNoTracking()
               .AsSplitQuery()
               .FirstAsync(x => x.Id == organization.Id, cancellationToken);

            await eventPublisher.PublishEvent(new OrganizationCreated(organization.Id, organization.Tenant.Id, organization.Name));

            return organization.ToDto();
        }
    }
}