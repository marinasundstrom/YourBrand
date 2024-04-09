
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.IdentityManagement.Application.Common.Interfaces;
using YourBrand.IdentityManagement.Application.Common.Models;

namespace YourBrand.IdentityManagement.Application.Tenants.Queries;

public record GetTenantsQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, IdentityManagement.Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<TenantDto>>
{
    public class GetTenantsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetTenantsQuery, ItemsResult<TenantDto>>
    {
        public async Task<ItemsResult<TenantDto>> Handle(GetTenantsQuery request, CancellationToken cancellationToken)
        {
            var query = context.Tenants
                .OrderBy(p => p.Created)
                .Skip(request.PageSize * request.Page)
                .Take(request.PageSize)
                .AsNoTracking()
                .AsSplitQuery();

            if (request.SearchString is not null)
            {
                query = query.Where(p =>
                    p.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalItems = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection == IdentityManagement.Application.Common.Models.SortDirection.Desc ? IdentityManagement.Application.SortDirection.Descending : IdentityManagement.Application.SortDirection.Ascending);
            }

            var tenants = await query
                .ToListAsync(cancellationToken);

            var dtos = tenants.Select(tenant => tenant.ToDto());

            return new ItemsResult<TenantDto>(dtos, totalItems);
        }
    }
}