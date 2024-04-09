using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.IdentityManagement.Domain.Exceptions;

namespace YourBrand.IdentityManagement.Application.Tenants.Commands;

public record UpdateTenantCommand(string TenantId, string Name) : IRequest<TenantDto>
{
    public sealed class UpdateUserDetailsCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<UpdateTenantCommand, TenantDto>
    {
        public async Task<TenantDto> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = await context.Tenants
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TenantId, cancellationToken);

            if (tenant is null)
            {
                throw new UserNotFoundException(request.TenantId);
            }

            tenant.ChangeName(request.Name);

            await context.SaveChangesAsync(cancellationToken);

            await eventPublisher.PublishEvent(new TenantUpdated(tenant.Id, tenant.Name));

            return tenant.ToDto();
        }
    }
}