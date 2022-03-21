
using YourCompany.Application.Common.Interfaces;
using YourCompany.Application.Common.Models;
using YourCompany.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourCompany.Application.Items.Queries;

public class GetItemsQuery : IRequest<Results<ItemDto>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; }
    public Application.Common.Models.SortDirection? SortDirection { get; }

    public GetItemsQuery(int page, int pageSize, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public class GetItemsQueryHandler : IRequestHandler<GetItemsQuery, Results<ItemDto>>
    {
        private readonly ICatalogContext context;
        private readonly IUrlHelper urlHelper;

        public GetItemsQueryHandler(ICatalogContext context, IUrlHelper urlHelper)
        {
            this.context = context;
            this.urlHelper = urlHelper;
        }

        public async Task<Results<ItemDto>> Handle(GetItemsQuery request, CancellationToken cancellationToken)
        {
            var query = context.Items
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .OrderBy(i => i.Created)
                .AsSplitQuery()
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(
                    request.SortBy,
                    request.SortDirection == Application.Common.Models.SortDirection.Desc ? Application.SortDirection.Descending : Application.SortDirection.Ascending);
            }

            query = query.Skip(request.Page * request.PageSize)
                    .Take(request.PageSize).AsQueryable();

            var items = await query.ToListAsync(cancellationToken);

            return new Results<ItemDto>(
                items.Select(item => item.ToDto(urlHelper)),
                totalCount);
        }
    }
}