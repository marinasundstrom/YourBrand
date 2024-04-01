
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Domain.Entities;

namespace YourBrand.IdentityManagement.Application.Tenants.Queries;

public record GetTenantQuery(string TenantId) : IRequest<TenantDto>
{
    public class GetTenantQueryHandler : IRequestHandler<GetTenantQuery, TenantDto>
    {
        private readonly IApplicationDbContext _context;

        public GetTenantQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TenantDto> Handle(GetTenantQuery request, CancellationToken cancellationToken)
        {
            var tenant = await _context.Tenants
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