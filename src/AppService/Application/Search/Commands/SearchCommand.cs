
using System.Data.Common;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Common.Models;
using YourBrand.Domain.Entities;
using YourBrand.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Application.Search.Commands;

public record SearchCommand(string SearchText,
        int Page, int PageSize, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<Results<SearchResultItem>>
{
    public class SearchCommandHandler : IRequestHandler<SearchCommand, Results<SearchResultItem>>
    {
        private readonly IAppServiceContext context;

        public SearchCommandHandler(IAppServiceContext context)
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