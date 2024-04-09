using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Exceptions;

namespace YourBrand.IdentityManagement.Application.Tenants.Commands;

public record DeleteTenantCommand(string TenantId) : IRequest
{
    public class DeleteUserCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<DeleteTenantCommand>
    {
        public async Task Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = await context.Tenants
                .FirstOrDefaultAsync(p => p.Id == request.TenantId);

            if (tenant is null)
            {
                throw new UserNotFoundException(request.TenantId);
            }

            context.Tenants.Remove(tenant);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new TenantDeleted(tenant.Id));

        }
    }
}