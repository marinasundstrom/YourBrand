using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Exceptions;

namespace YourBrand.IdentityManagement.Application.Organizations.Commands;

public record DeleteOrganizationCommand(string OrganizationId) : IRequest
{
    public sealed class DeleteUserCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<DeleteOrganizationCommand>
    {
        public async Task Handle(DeleteOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await context.Organizations
                .FirstOrDefaultAsync(p => p.Id == request.OrganizationId);

            if (organization is null)
            {
                throw new UserNotFoundException(request.OrganizationId);
            }

            context.Organizations.Remove(organization);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new OrganizationDeleted(organization.Id));

        }
    }
}