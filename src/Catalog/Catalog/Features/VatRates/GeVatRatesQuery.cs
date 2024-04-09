using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog;
using YourBrand.Catalog.Model;
using YourBrand.Catalog.Persistence;
using YourBrand.Identity;

namespace YourBrand.Catalog.Features.VatRates;

public sealed record GetVatRatesQuery(int Page = 1, int PageSize = 10, string? SearchString = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<VatRateDto>>
{
    sealed class GetVatRatesQueryHandler(
        CatalogContext context) : IRequestHandler<GetVatRatesQuery, PagedResult<VatRateDto>>
    {
        public async Task<PagedResult<VatRateDto>> Handle(GetVatRatesQuery request, CancellationToken cancellationToken)
        {
            var query = context.VatRates
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
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync();

            return new PagedResult<VatRateDto>(items.Select(item => item.ToDto()),
            totalCount);
        }
    }
}