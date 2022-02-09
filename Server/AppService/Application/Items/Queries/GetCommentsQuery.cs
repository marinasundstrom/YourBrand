
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using Catalog.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Items.Queries;

public class GetCommentsQuery : IRequest<Results<CommentDto>>
{
    public string ItemId { get; set; }

    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; }
    public Application.Common.Models.SortDirection? SortDirection { get; }

    public GetCommentsQuery(string itemId, int page, int pageSize, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        ItemId = itemId;
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, Results<CommentDto>>
    {
        private readonly ICatalogContext context;

        public GetCommentsQueryHandler(ICatalogContext context)
        {
            this.context = context;
        }

        public async Task<Results<CommentDto>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
        {
            var query = context.Comments
                .Where(c => c.Item.Id == request.ItemId)
                .OrderByDescending(c => c.Created)
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

            var comments = await query.ToListAsync(cancellationToken);

            return new Results<CommentDto>(
                comments.Select(comment => new CommentDto(comment.Id, comment.Text, comment.Created, comment.CreatedBy, comment.LastModified, comment.LastModifiedBy)),
                totalCount);
        }
    }
}