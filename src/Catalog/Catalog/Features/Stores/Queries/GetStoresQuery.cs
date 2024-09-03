using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog;
using YourBrand.Catalog.Model;
using YourBrand.Catalog.Persistence;
using YourBrand.Identity;

namespace YourBrand.Catalog.Features.Stores.Queries;

public sealed record GetStoresQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<StoreDto>>
{
    sealed class GetStoresQueryHandler(
        CatalogContext context) : IRequestHandler<GetStoresQuery, PagedResult<StoreDto>>
    {
        public async Task<PagedResult<StoreDto>> Handle(GetStoresQuery request, CancellationToken cancellationToken)
        {
            var query = context.Stores
                .InOrganization(request.OrganizationId)
                .Include(x => x.Currency)
                .AsSplitQuery()
                .AsNoTracking()
                .AsQueryable();

            if (request.SearchString is not null)
            {
                query = query.Where(ca => ca.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await query.CountAsync();

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderBy(x => x.Name);
            }

            var items = await query
                .InOrganization(request.OrganizationId)
                .Include(x => x.Currency)
                .Skip(request.Page * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync();

            return new PagedResult<StoreDto>(items.Select(item => item.ToDto()),
            totalCount);
        }
    }
}