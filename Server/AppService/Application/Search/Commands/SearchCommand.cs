
using System.Data.Common;

using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using Catalog.Domain.Entities;
using Catalog.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Search.Commands;

public class SearchCommand : IRequest<Results<SearchResultItem>>
{
    public string SearchText { get; set; } = null!;

    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; }
    public Application.Common.Models.SortDirection? SortDirection { get; }

    public SearchCommand(string searchText,
        int page, int pageSize, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        SearchText = searchText;
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public class SearchCommandHandler : IRequestHandler<SearchCommand, Results<SearchResultItem>>
    {
        private readonly ICatalogContext context;

        public SearchCommandHandler(ICatalogContext context)
        {
            this.context = context;
        }

        public async Task<Results<SearchResultItem>> Handle(SearchCommand request, CancellationToken cancellationToken)
        {
            var searchText = request.SearchText.Trim().ToLower();

            var query = context.Items.Where(i =>
                i.Name.Trim().ToLower().Contains(searchText)
                || i.Description.Trim().ToLower().Contains(searchText))
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            var projectedQuery = query.Select(i => new SearchResultItem()
            {
                Title = i.Name,
                ResultType = SearchResultItemType.Item,
                ItemId = i.Id
            });

            if (request.SortBy is not null)
            {
                projectedQuery = projectedQuery.OrderBy(
                    request.SortBy,
                    request.SortDirection == Application.Common.Models.SortDirection.Desc ? Application.SortDirection.Descending : Application.SortDirection.Ascending);
            }

            projectedQuery = projectedQuery.Skip(request.Page * request.PageSize)
                .Take(request.PageSize).AsQueryable();

            var resultItems = await projectedQuery.ToListAsync(cancellationToken);

            return new Results<SearchResultItem>(
                resultItems,
                totalCount);
        }
    }
}