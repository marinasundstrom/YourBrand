using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Domain.Exceptions;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Application.Organizations.Commands;

public record DeleteOrganizationCommand(string OrganizationId) : IRequest
{
    public class DeletePersonCommandHandler(IApplicationDbContext context, IUserContext currentPersonService, IEventPublisher eventPublisher) : IRequestHandler<DeleteOrganizationCommand>
    {
        public async Task Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations
                .FirstOrDefaultAsync(p => p.Id == request.OrganizationId);

            if (organization is null)
            {
                throw new PersonNotFoundException(request.OrganizationId);
            }

            context.Organizations.Remove(organization);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new OrganizationDeleted(organization.Id));

        }
    }
}