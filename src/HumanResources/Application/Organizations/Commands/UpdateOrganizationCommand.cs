using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Organizations.Commands;

public record UpdateOrganizationCommand(string OrganizationId, string Name) : IRequest<OrganizationDto>
{
    public class UpdatePersonDetailsCommandHandler(IApplicationDbContext context, IUserContext currentPersonService, IEventPublisher eventPublisher) : IRequestHandler<UpdateOrganizationCommand, OrganizationDto>
    {
        public async Task<OrganizationDto> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.OrganizationId, cancellationToken);

            if (organization is null)
            {
                throw new PersonNotFoundException(request.OrganizationId);
            }

            organization.ChangeName(request.Name);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new OrganizationUpdated(organization.Id, organization.Name));

            return organization.ToDto();
        }
    }
}