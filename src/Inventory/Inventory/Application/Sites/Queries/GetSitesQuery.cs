using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Application.Sites.Queries;

public record GetSitesQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<ItemsResult<SiteDto>>
{
    sealed class GetSitesQueryHandler(
        IInventoryContext context,
        IUserContext userContext) : IRequestHandler<GetSitesQuery, ItemsResult<SiteDto>>
    {
        public async Task<ItemsResult<SiteDto>> Handle(GetSitesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<Site> result = context
                    .Sites
                    .OrderBy(o => o.Created)
                    .AsNoTracking()
                    .AsQueryable();

            if (request.SearchString is not null)
            {
                result = result.Where(o => o.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection == Application.Common.Models.SortDirection.Desc ? Inventory.Application.SortDirection.Descending : Inventory.Application.SortDirection.Ascending);
            }
            else
            {
                result = result.OrderBy(x => x.Name);
            }

            var items = await result
                .Skip((request.Page) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new ItemsResult<SiteDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}