using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog;
using YourBrand.Catalog.Model;
using YourBrand.Catalog.Persistence;
using YourBrand.Identity;

namespace YourBrand.Catalog.Features.Currencies;

public sealed record GetCurrenciesQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<CurrencyDto>>
{
    sealed class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, PagedResult<CurrencyDto>>
    {
        private readonly CatalogContext _context;
        private readonly IUserContext userContext;

        public GetCurrenciesQueryHandler(
            CatalogContext context,
            IUserContext userContext)
        {
            _context = context;
            this.userContext = userContext;
        }

        public async Task<PagedResult<CurrencyDto>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Currencies
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
                query = query.OrderBy(x => x.Symbol);
            }

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync();

            return new PagedResult<CurrencyDto>(items.Select(item => item.ToDto()),
            totalCount);
        }
    }
}