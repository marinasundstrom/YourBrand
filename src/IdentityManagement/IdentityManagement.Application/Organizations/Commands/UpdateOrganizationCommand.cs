using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Exceptions;

namespace YourBrand.IdentityManagement.Application.Organizations.Commands;

public record UpdateOrganizationCommand(string OrganizationId, string Name) : IRequest<OrganizationDto>
{
    public sealed class UpdateUserDetailsCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
    {
        public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if (organization is null)
            {
                throw new UserNotFoundException(request.OrganizationId);
            }

            organization.ChangeName(request.Name);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new OrganizationUpdated(organization.Id, organization.Name));

            return organization.ToDto();
        }
    }
}