
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityManagement.Application.Common.Interfaces;

namespace YourBrand.IdentityManagement.Application.Tenants.Queries;

public record GetTenantQuery(string TenantId) : IRequest<TenantDto>
{
    public class GetTenantQueryHandler(IApplicationDbContext context) : IRequestHandler<GetTenantQuery, TenantDto>
    {
        public async Task<TenantDto> Handle(GetTenantQuery request, CancellationToken cancellationToken)
        {
            var tenant = await context.Tenants
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.TenantId, cancellationToken);

            if (tenant is null)
            {
                return null!;
            }

            return tenant.ToDto();
        }
    }
}