using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Domain.Entities;

using TenantCreated = YourBrand.IdentityManagement.Contracts.TenantCreated;

namespace YourBrand.IdentityManagement.Application.Tenants.Commands;

public record CreateTenantCommand(string Name, string? FriendlyName) : IRequest<TenantDto>
{
    public class CreateTenantCommandHandler(IApplicationDbContext context, IEventPublisher eventPublisher) : IRequestHandler<CreateTenantCommand, TenantDto>
    {
        public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenant = new Tenant(
                 request.Name,
                 request.FriendlyName ?? request.Name.ToLower().Replace(' ', '-'));

            context.Tenants.Add(tenant);

            await context.SaveChangesAsync(cancellationToken);

            tenant = await context.Tenants
               .AsNoTracking()
               .AsSplitQuery()
               .FirstAsync(x => x.Id == tenant.Id, cancellationToken);

            await eventPublisher.PublishEvent(new TenantCreated(tenant.Id, tenant.Name));

            return tenant.ToDto();
        }
    }
}